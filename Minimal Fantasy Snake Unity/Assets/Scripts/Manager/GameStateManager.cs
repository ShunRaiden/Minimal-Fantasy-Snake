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

    public class SetUpState : ICommand
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || SetUpState");

            var gm = GameManager.instance;

            yield return new WaitUntil(() => gm.gridManager.GenerateGrid());
            yield return new WaitUntil(() => gm.SetUpGameState());

            gm.SetState(gm.GameState.inputState);
        }
    }

    public class InputState : ICommand
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

    public class WalkState : ICommand
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
                        gm.CollectHero(charInGrid);
                        gm.SetState(gm.GameState.inputState);
                        break;
                    case CharacterType.Monster:
                        gm.currentMonster = charInGrid;
                        gm.gameplayUIManager.SetUpMonsterGameplayUI(gm.currentMonster.statusCharacter);
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
                }
                else
                {
                    gm.SetState(gm.GameState.inputState);
                }
            }
        }
    }

    public class CombatState : ICommand
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || CombatState");

            var gm = GameManager.instance;

            yield return gm.playerManager.combatCameraManager.StartCombatCamera();

            gm.currentHero = gm.playerManager.TryGetFirstHero();            

            int turnCount = 0;

            while (!gm.currentHero.isDead && !gm.currentMonster.isDead)
            {
                yield return gm.PlayerAttack();

                if (gm.currentMonster.isDead) break;

                yield return gm.MonsterAttack();

                if (gm.currentHero.isDead) break;

                turnCount++;

                if (turnCount >= gm.turnPerCombatLitmit)
                {
                    yield return gm.DrawCondition();
                    break;
                }
            }           

            yield return null;
        }
    }

    public class PostCombatState : ICommand
    {
        public IEnumerator Execute()
        {
            Debug.Log($"Start State || PostCombatState");

            var gm = GameManager.instance;

            if (gm.playerManager.CheckHeroRemaining() > 0)
            {
                gm.gameplayUIManager.SetUpPlayerGameplayUI(gm.playerManager.TryGetFirstHero().statusCharacter);
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

    public class GameOverState : ICommand
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
