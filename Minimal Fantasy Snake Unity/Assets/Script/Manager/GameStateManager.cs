using Character;
using System.Collections;
using UnityEngine;

namespace Manager
{
    public class GameStateManager
    {
        public Coroutine currentState;

        public SetUpState setUpState;
        public InputState inputState;
        public WalkState walkState;
        public CombatState combatState;
        public PostCombatState postCombatState;
        public GameOverState gameOverState;

        public GameStateManager()
        {
            setUpState = new SetUpState();
            inputState = new InputState();
            walkState = new WalkState();
            combatState = new CombatState();
            postCombatState = new PostCombatState();
            gameOverState = new GameOverState();
        }
    }

    public class SetUpState : IGameState
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || SetUpState");

            var gm = GameManager.instance;

            yield return new WaitUntil(() => gm.gridManager.GenerateGrid());

            gm.navMeshSurface.BuildNavMesh();
            gm.playerManager.SetUpPlayer();
            gm.gameplayUIManager.SetUpPlayerGameplayUI(gm.playerManager.TryGetFirstHero().statusCharacter);
            gm.gameplayUIManager.UpdatePlayerCount();

            for (int i = 0; i < gm.startSpawnCount; i++)
            {
                gm.SpawnCharacter();
            }

            gm.SetState(gm.GameState.inputState);
        }
    }

    public class InputState : IGameState
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || InputState");

            var gm = GameManager.instance;
            gm.playerManager.canInput = true;

            yield return new WaitUntil(() => !gm.playerManager.canInput);

            gm.SetState(gm.GameState.walkState);
        }
    }

    public class WalkState : IGameState
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || WalkState");

            var gm = GameManager.instance;
            var charInGrid = gm.CheckHasCharacter();

            if (charInGrid != null)
            {
                switch (gm.CheckTypeCharacter())
                {
                    case CharacterType.Hero:
                        gm.DeleteCharacterOnGrid();
                        yield return gm.playerManager.MoveAllHeroNormal();
                        gm.playerManager.CollectedHero(charInGrid.statusCharacter.GetDataSetup());
                        gm.SpawnCharacter();
                        gm.gameplayUIManager.UpdatePlayerCount();
                        gm.SetState(gm.GameState.inputState);
                        break;
                    case CharacterType.Monster:
                        gm.currentMonster = charInGrid;
                        gm.SetState(gm.GameState.combatState);
                        gm.gameplayUIManager.SetUpMonsterGameplayUI(gm.currentMonster.statusCharacter);
                        break;
                }
            }
            else
            {
                yield return gm.playerManager.MoveAllHeroNormal();

                if (gm.OnCheckLoseMove())
                {
                    gm.SetState(gm.GameState.gameOverState);
                    Debug.Log("Game Over by : Wrong Move");
                    yield break;
                }

                gm.SetState(gm.GameState.inputState);
            }
        }
    }

    public class CombatState : IGameState
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || CombatState");

            var gm = GameManager.instance;

            gm.currentHero = gm.playerManager.TryGetFirstHero();

            int turnCount = 0;

            while (!gm.currentHero.isDead && !gm.currentMonster.isDead)
            {
                gm.currentHero.Attack(gm.currentMonster);
                gm.gameplayUIManager.UpdateMonsterSlot(gm.currentMonster.statusCharacter);

                yield return new WaitForSeconds(gm.currentHero.animationCharacter.attackTiming);

                if (gm.currentMonster.isDead)
                {
                    gm.DeleteCharacterOnGrid();
                    yield return gm.playerManager.MoveAllHeroNormal();
                    gm.gameplayUIManager.RemoveMonsterProfile();
                    gm.SpawnCharacter();
                    gm.SetState(gm.GameState.inputState);
                    break;
                }

                gm.currentMonster.Attack(gm.currentHero);
                gm.gameplayUIManager.UpdatePlayerSlot(gm.currentHero.statusCharacter);

                yield return new WaitForSeconds(gm.currentMonster.animationCharacter.attackTiming);

                if (gm.currentHero.isDead)
                {
                    gm.tileHeroToMoveInto = gm.playerManager.currentPlayerHero[(HeroManager)gm.currentHero];
                    gm.gameplayUIManager.RemovePlayerProfile();
                    gm.playerManager.DeletePlayerHeroHasDead();
                    gm.gameplayUIManager.UpdatePlayerCount();
                    gm.SetState(gm.GameState.postCombatState);
                    break;
                }

                turnCount++;

                if (turnCount >= gm.turnPerCombatLitmit)
                {
                    gm.currentHero.TakeDamage(999);
                    gm.gameplayUIManager.UpdateMonsterSlot(gm.currentMonster.statusCharacter);

                    gm.currentMonster.TakeDamage(999);
                    gm.gameplayUIManager.UpdatePlayerSlot(gm.currentHero.statusCharacter);
                    gm.tileHeroToMoveInto = gm.playerManager.currentPlayerHero[(HeroManager)gm.currentHero];

                    gm.gameplayUIManager.RemovePlayerProfile();
                    gm.playerManager.DeletePlayerHeroHasDead();
                    gm.gameplayUIManager.UpdatePlayerCount();

                    gm.SetState(gm.GameState.postCombatState);
                }
            }

            yield return null;
        }
    }

    public class PostCombatState : IGameState
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || PostCombatState");

            var gm = GameManager.instance;


            if (gm.playerManager.CheckHeroRemaining() > 0)
            {
                if (gm.currentMonster.isDead) // If Draw
                {
                    gm.DeleteCharacterOnGrid();
                    yield return gm.playerManager.MoveAllHeroNormal();
                    gm.gameplayUIManager.RemoveMonsterProfile();
                    gm.SpawnCharacter();
                    gm.SetState(gm.GameState.inputState);
                    gm.gameplayUIManager.SetUpPlayerGameplayUI(gm.playerManager.TryGetFirstHero().statusCharacter);
                }
                else
                {
                    gm.gameplayUIManager.SetUpPlayerGameplayUI(gm.playerManager.TryGetFirstHero().statusCharacter);
                    yield return gm.playerManager.MoveAllHeroWithTile(gm.tileHeroToMoveInto);
                    gm.SetState(gm.GameState.combatState);
                }
            }
            else
            {
                gm.SetState(gm.GameState.gameOverState);
                Debug.Log("Game Over by : Lose Fight");
            }

            yield return null;
        }
    }

    public class GameOverState : IGameState
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || GameOverState");

            var gm = GameManager.instance;
            gm.OnGameOver();
            yield return null;
        }
    }
}