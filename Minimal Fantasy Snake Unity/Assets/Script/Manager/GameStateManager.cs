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
            var gm = GameManager.instance;

            yield return new WaitUntil(() => gm.gridManager.GenerateGrid());

            gm.navMeshSurface.BuildNavMesh();
            gm.playerManager.SetUpPlayer();

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
            var gm = GameManager.instance;
            var charInGrid = gm.CheckHasCharacter();

            if (charInGrid != null)
            {
                switch (gm.CheckTypeCharacter())
                {
                    case CharacterType.Hero:
                        gm.DeleteCharacterOnGrid();
                        yield return gm.playerManager.MoveAllHeroNormal();
                        gm.playerManager.CollectedHero();
                        gm.SpawnCharacter();
                        gm.SetState(gm.GameState.inputState);
                        break;
                    case CharacterType.Monster:
                        gm.currentMonster = charInGrid;
                        gm.SetState(gm.GameState.combatState);
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
            var gm = GameManager.instance;

            gm.currentHero = gm.playerManager.TryGetFirstHero();

            while (!gm.currentHero.isDead && !gm.currentMonster.isDead)
            {
                gm.currentHero.Attack(gm.currentMonster);

                yield return new WaitForSeconds(gm.currentHero.animationCharacter.attackTiming);

                if (gm.currentMonster.isDead)
                {
                    gm.DeleteCharacterOnGrid();
                    yield return gm.playerManager.MoveAllHeroNormal();
                    gm.SpawnCharacter();
                    gm.SetState(gm.GameState.inputState);
                    break;
                }

                gm.currentMonster.Attack(gm.currentHero);

                yield return new WaitForSeconds(gm.currentMonster.animationCharacter.attackTiming);

                if (gm.currentHero.isDead)
                {
                    gm.tileHeroToMoveInto = gm.playerManager.currentPlayerHero[(HeroManager)gm.currentHero];
                    gm.playerManager.DeletePlayerHeroHasDead();
                    gm.SetState(gm.GameState.postCombatState);
                    break;
                }
            }

            yield return null;
        }
    }

    public class PostCombatState : IGameState
    {
        public IEnumerator Execute()
        {
            var gm = GameManager.instance;

            if (gm.playerManager.CheckHeroRemaining())
            {
                yield return gm.playerManager.MoveAllHeroWithTile(gm.tileHeroToMoveInto);
                gm.SetState(gm.GameState.combatState);
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
            var gm = GameManager.instance;
            gm.OnGameOver();
            yield return null;
        }
    }
}