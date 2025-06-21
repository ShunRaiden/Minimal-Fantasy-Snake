using Character;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.AI.Navigation;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager instance { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            if (instance == null)
            {
                instance = this;
                InitializeUI();
            }
        }

        private void OnDestroy()
        {
            DisposeUI();
        }
        #endregion

        public event Action OnStartGameEvent;
        public event Action OnGameOverEvent;

        [Header("UI")]
        public UIMainMenuPanel uiMainMenuPanel;
        public UIGameOverPanel uiGameOverPanel;
        public UIGameplaysPanel uiGamePlaysPanel;

        public GameStateManager GameState;
        public GameplayUIManager gameplayUIManager;
        public GridManager gridManager;
        public PlayerManager playerManager;
        public NavMeshSurface navMeshSurface;

        [Header("Spawn")]
        public List<CharacterManager> characterPrefab = new List<CharacterManager>();
        public int startSpawnCount = 2;

        [Header("Combat Data")]
        public int turnPerCombatLitmit = 20;
        public CharacterManager currentMonster;
        public CharacterManager currentHero;
        public Tile tileHeroToMoveInto;
        public Dictionary<Tile, CharacterManager> currentCharacterOnGrid = new Dictionary<Tile, CharacterManager>();

        #region State SetUp
        public void SetUpMainMenu()
        {
            AudioManager.instance.StopMusic();

            uiMainMenuPanel?.OpenPanel();
            uiGameOverPanel?.ClosePanel();
            ClearCharacter();
        }

        public void OnStartGame()
        {
            AudioManager.instance.PlayMusic(AudioManager.BASE_BGM);

            if (GameState == null)
            {
                GameState = new GameStateManager();
            }

            ClearCharacter();

            if (GameState != null)
            {
                uiMainMenuPanel?.ClosePanel();
                SetState(GameState.setUpState);
            }

            OnStartGameEvent?.Invoke();
        }

        public void SetState(IGameState newState)
        {
            if (GameState.currentState != null)
            {
                StopCoroutine(GameState.currentState);
            }

            GameState.currentState = StartCoroutine(newState.Execute());
        }

        private void ClearCharacter()
        {
            currentHero = null;
            currentMonster = null;
            tileHeroToMoveInto = null;

            if (currentCharacterOnGrid.Count > 0)
            {
                foreach (var hero in currentCharacterOnGrid.Values)
                {
                    Destroy(hero.gameObject);
                }

                currentCharacterOnGrid.Clear();
            }

            playerManager.ResetPlayer();
            gameplayUIManager.ClearGameplaySlot();
        }
        #endregion

        #region Character
        public void SpawnCharacter()
        {
            // All Avaiable Pos
            List<Vector2> availablePositions = new List<Vector2>();

            for (int x = 0; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height; y++)
                {
                    Vector2 gridPos = new Vector2(x, y);

                    if (CanSpawn(gridPos))
                    {
                        availablePositions.Add(gridPos);
                    }
                }
            }

            if (availablePositions.Count == 0)
            {
                SetState(GameState.gameOverState);
                return;
            }

            // Random Tile
            Vector2 spawnGridPos = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];

            Tile tile = gridManager.gridTiles[spawnGridPos];

            CharacterManager newChar = Instantiate(characterPrefab[UnityEngine.Random.Range(0, characterPrefab.Count)],
                                                   tile.transform.position, Quaternion.identity);

            newChar.RandomSetUp();

            currentCharacterOnGrid.Add(tile, newChar);
        }

        public Dictionary<Vector2, CharacterManager> GetAllCharacterPositions()
        {
            var allCharacterPos = new Dictionary<Vector2, CharacterManager>();

            foreach (var hero in playerManager.GetHeroPositions())
            {
                allCharacterPos[hero.Key] = hero.Value;
            }

            foreach (var monster in currentCharacterOnGrid)
            {
                allCharacterPos[monster.Key.GridPosition] = monster.Value;
            }

            return allCharacterPos;
        }

        public bool CanSpawn(Vector2 targetGridPos)
        {
            return !GetAllCharacterPositions().ContainsKey(targetGridPos);
        }

        public CharacterManager CheckHasCharacter()
        {
            if (currentCharacterOnGrid.ContainsKey(playerManager.currentPostion))
            {
                return currentCharacterOnGrid[playerManager.currentPostion];
            }
            else
            {
                return null;
            }
        }

        public CharacterType CheckTypeCharacter()
        {
            return currentCharacterOnGrid[playerManager.currentPostion].characterType;
        }

        public bool OnCheckLoseMove()
        {
            bool isFirst = true;

            foreach (var tile in playerManager.currentPlayerHero.Values)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                if (tile.GridPosition == playerManager.currentPostion.GridPosition)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteCharacterOnGrid()
        {
            var character = currentCharacterOnGrid[playerManager.currentPostion];
            currentCharacterOnGrid.Remove(playerManager.currentPostion);
            Destroy(character.gameObject);
        }
        #endregion

        #region UI
        public void InitializeUI()
        {
            uiGamePlaysPanel.UpdateGameSpeedText();

            uiMainMenuPanel.OnStartButtonClickEvent += OnStartGame;
            uiMainMenuPanel.OnQuitButtonClickEvent += OnQuitGmae;
            uiGameOverPanel.OnRestartButtonClickEvent += OnStartGame;
            uiGameOverPanel.OnMianMenuButtonClickEvent += SetUpMainMenu;
            uiGamePlaysPanel.OnGameSpeedButtonClickEvent += SetUpGameSpeed;
        }

        public void DisposeUI()
        {
            uiMainMenuPanel.OnStartButtonClickEvent -= OnStartGame;
            uiMainMenuPanel.OnQuitButtonClickEvent -= OnQuitGmae;
            uiGameOverPanel.OnRestartButtonClickEvent -= OnStartGame;
            uiGameOverPanel.OnMianMenuButtonClickEvent -= SetUpMainMenu;
            uiGamePlaysPanel.OnGameSpeedButtonClickEvent -= SetUpGameSpeed;
        }

        public void OnGameOver()
        {
            OnGameOverEvent?.Invoke();
        }

        public void SetUpGameSpeed(int speed)
        {
            Time.timeScale = speed;
            uiGamePlaysPanel.UpdateGameSpeedText();
        }

        public int TryGetGameSpeedInt()
        {
            return (int)Time.timeScale;
        }

        public void OnQuitGmae()
        {
            Application.Quit();
        }
        #endregion

        #region StateMethod
        public bool SetUpGameState()
        {
            navMeshSurface.BuildNavMesh();
            playerManager.SetUpPlayer();
            gameplayUIManager.SetUpPlayerGameplayUI(playerManager.TryGetFirstHero().statusCharacter);
            gameplayUIManager.UpdatePlayerCount();

            for (int i = 0; i < startSpawnCount; i++)
            {
                SpawnCharacter();
            }

            return true;
        }

        public void CollectHero(CharacterManager charInGrid)
        {
            playerManager.CollectedHero(charInGrid.statusCharacter.GetDataSetup());
            SpawnCharacter();
            gameplayUIManager.UpdatePlayerCount();
        }

        public IEnumerator PlayerAttack()
        {
            gameplayUIManager.PlayPlayerSlotAttackAnimation();
            yield return currentHero.Attack(currentMonster);
            gameplayUIManager.UpdateMonsterSlot(currentMonster.statusCharacter);

            yield return new WaitForSeconds(currentHero.animationCharacter.attackTiming);

            if (currentMonster.isDead)
            {
                DeleteCharacterOnGrid();
                yield return playerManager.MoveAllHeroNormal();
                gameplayUIManager.RemoveMonsterProfile();
                SpawnCharacter();
                SetState(GameState.inputState);
            }
        }

        public IEnumerator MonsterAttack()
        {
            gameplayUIManager.PlayMonsterSlotAttackAnimation();
            yield return currentMonster.Attack(currentHero);
            gameplayUIManager.UpdatePlayerSlot(currentHero.statusCharacter);

            yield return new WaitForSeconds(currentMonster.animationCharacter.attackTiming);

            if (currentHero.isDead)
            {
                tileHeroToMoveInto = playerManager.currentPlayerHero[(HeroManager)currentHero];
                gameplayUIManager.RemovePlayerProfile();
                playerManager.DeletePlayerHeroHasDead();
                gameplayUIManager.UpdatePlayerCount();
                SetState(GameState.postCombatState);
            }
        }

        public IEnumerator DrawCondition()
        {
            currentHero.TakeDamage(999);
            gameplayUIManager.UpdateMonsterSlot(currentMonster.statusCharacter);

            currentMonster.TakeDamage(999);
            gameplayUIManager.UpdatePlayerSlot(currentHero.statusCharacter);
            tileHeroToMoveInto = playerManager.currentPlayerHero[(HeroManager)currentHero];

            gameplayUIManager.RemovePlayerProfile();
            playerManager.DeletePlayerHeroHasDead();
            gameplayUIManager.UpdatePlayerCount();

            if (playerManager.CheckHeroRemaining() > 0)
            {
                DeleteCharacterOnGrid();
                yield return playerManager.MoveAllHeroNormal();
                gameplayUIManager.RemoveMonsterProfile();
                SpawnCharacter();
                gameplayUIManager.SetUpPlayerGameplayUI(playerManager.TryGetFirstHero().statusCharacter);
                SetState(GameState.inputState);
            }
            else
            {
                SetState(GameState.gameOverState);
                Debug.Log("Game Over by : Lose Fight");
            }
        }
        #endregion
    }
}