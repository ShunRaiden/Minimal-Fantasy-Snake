using Character;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [Header("Manager")]
        public GameStateManager GameState;
        public GameplayUIManager gameplayUIManager;
        public GridManager gridManager;
        public NavMeshSurface navMeshSurface;
        public PlayerManager playerManager;

        [Header("Spawn")]
        public List<HeroManager> heroPerfab = new List<HeroManager>();
        public List<CharacterManager> characterPrefab = new List<CharacterManager>();
        public List<BuffItem> buffITemPrefab = new List<BuffItem>();
        public int startCharacterSpawnCount = 2;
        public int startItemSpawnCount = 2;
        [Range(0, 100)] public float spawnRate;

        [Header("Combat Data")]
        public int turnPerCombatLitmit = 10;

        [Header("Debug")]
        public CharacterManager currentMonster;
        public CharacterManager currentHero;
        public Tile tileHeroToMoveInto;
        public Dictionary<Tile, CharacterManager> currentCharacterOnGrid = new Dictionary<Tile, CharacterManager>();
        public Dictionary<Tile, BuffItem> currentBuffItemOnGrid = new Dictionary<Tile, BuffItem>();

        #region State SetUp
        public void SetUpMainMenu()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnStartGame()
        {
            AudioManager.instance.PlayMusic(AudioManager.BASE_BGM);

            if (GameState == null)
            {
                GameState = new GameStateManager();
            }

            ClearCharacter();
            ClearBuff();

            if (GameState != null)
            {
                uiMainMenuPanel?.ClosePanel();
                SetState(GameState.setUpState);
            }

            OnStartGameEvent?.Invoke();
        }

        public void SetState(ICommand newState)
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

        private void ClearBuff()
        {
            if (currentBuffItemOnGrid.Count > 0)
            {
                foreach (var hero in currentBuffItemOnGrid.Values)
                {
                    Destroy(hero.gameObject);
                }

                currentBuffItemOnGrid.Clear();
            }
        }
        #endregion

        #region Character
        public void SpawnCharacter()
        {
            List<Vector2> availablePositions = AvailablePositions();

            if (availablePositions.Count == 0)
            {
                SetState(GameState.gameOverState);
                return;
            }

            Vector2 spawnGridPos = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];

            Tile tile = gridManager.gridTiles[spawnGridPos];

            CharacterManager newChar = Instantiate(characterPrefab[UnityEngine.Random.Range(0, characterPrefab.Count)],
                                                   tile.transform.position, Quaternion.identity);

            newChar.RandomSetUp();

            currentCharacterOnGrid.Add(tile, newChar);
        }

        public void RandomSpawnBuffItem()
        {
            List<Vector2> availablePositions = AvailablePositions();

            if (availablePositions.Count == 0)
            {
                SetState(GameState.gameOverState);
                return;
            }

            Vector2 spawnGridPos = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];

            Tile tile = gridManager.gridTiles[spawnGridPos];

            BuffItem newItem = Instantiate(buffITemPrefab[UnityEngine.Random.Range(0, buffITemPrefab.Count)],
                                           tile.transform.position, Quaternion.identity);

            currentBuffItemOnGrid.Add(tile, newItem);
        }

        private List<Vector2> AvailablePositions()
        {
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

            return availablePositions;
        }

        private Dictionary<Vector2, CharacterManager> GetAllCharacterPositions()
        {
            var allCharacterPos = new Dictionary<Vector2, CharacterManager>();

            foreach (var hero in playerManager.GetHeroPositions())
            {
                allCharacterPos[hero.Key] = hero.Value;
            }

            foreach (var monster in currentCharacterOnGrid)
            {
                allCharacterPos[monster.Key.gridPosition] = monster.Value;
            }

            return allCharacterPos;
        }

        private Dictionary<Vector2, BuffItem> GetAllBuffPostion()
        {
            var allItemPos = new Dictionary<Vector2, BuffItem>();

            foreach (var item in currentBuffItemOnGrid)
            {
                allItemPos[item.Key.gridPosition] = item.Value;
            }

            return allItemPos;
        }

        public bool CanSpawn(Vector2 targetGridPos)
        {
            return !GetAllCharacterPositions().ContainsKey(targetGridPos)
                && !GetAllBuffPostion().ContainsKey(targetGridPos);
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

        public BuffItem CheckHasBuffItem()
        {
            if (currentBuffItemOnGrid.ContainsKey(playerManager.currentPostion))
            {
                return currentBuffItemOnGrid[playerManager.currentPostion];
            }
            else
            {
                return null;
            }
        }

        public bool RandomHasSpawnItem()
        {
            return UnityEngine.Random.Range(0, 100) < spawnRate;
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

                if (tile.gridPosition == playerManager.currentPostion.gridPosition)
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
            AudioManager.instance.StopMusic();
            AudioManager.instance.PlayOneShotSFX(AudioManager.GAME_LOSE);

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

            for (int i = 0; i < startCharacterSpawnCount; i++)
            {
                SpawnCharacter();
            }

            for (int i = 0; i < startItemSpawnCount; i++)
            {
                RandomSpawnBuffItem();
            }

            currentHero = playerManager.TryGetFirstHero();

            return true;
        }

        public void CollectHero(CharacterManager charInGrid)
        {
            playerManager.CollectedHero(charInGrid);
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
                yield return playerManager.combatCameraManager.StopCombatCamera();
                DeleteCharacterOnGrid();
                yield return playerManager.MoveAllHeroNormal();
                gameplayUIManager.RemoveMonsterProfile();
                SpawnCharacter();

                if (RandomHasSpawnItem())
                {
                    RandomSpawnBuffItem();
                }
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
                yield return playerManager.combatCameraManager.StopCombatCamera();
                tileHeroToMoveInto = playerManager.currentPlayerHero[(HeroManager)currentHero];
                gameplayUIManager.RemovePlayerProfile();
                playerManager.DeletePlayerHeroHasDead();
                gameplayUIManager.UpdatePlayerCount();
                SetState(GameState.postCombatState);
            }
        }

        public bool CheckDrawCondition(CharacterBaseStatus hero, CharacterBaseStatus monster)
        {
            return hero.currentDEF >= monster.currentATK
                && monster.currentDEF >= hero.currentATK;
        }

        public IEnumerator StartDrawCondition()
        {
            currentHero.TakeDamage(999, CharacterClass.God);
            gameplayUIManager.UpdateMonsterSlot(currentMonster.statusCharacter);

            currentMonster.TakeDamage(999, CharacterClass.God);
            gameplayUIManager.UpdatePlayerSlot(currentHero.statusCharacter);
            tileHeroToMoveInto = playerManager.currentPlayerHero[(HeroManager)currentHero];

            gameplayUIManager.RemovePlayerProfile();
            playerManager.DeletePlayerHeroHasDead();
            gameplayUIManager.UpdatePlayerCount();

            if (playerManager.CheckHeroRemaining() > 0)
            {
                yield return playerManager.combatCameraManager.StopCombatCamera();
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