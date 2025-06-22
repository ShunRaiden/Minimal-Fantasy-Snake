using Character;
using Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Paramiter")]
        public HeroManager scissorPrefab;
        public HeroManager paperPrefab;
        public HeroManager rockPrefab;

        public PlayerInputManager inputManager;
        public CombatCameraManager combatCameraManager;

        public Vector2 startPos = new Vector2(0, 0);

        public GameObject playerTile;

        public string collectSoundKey = "CollectHero";
        public string moveSoundKey = "Move";

        [Header("Debug")]
        public Tile currentPostion;
        public Tile lastTileOfPlayer;

        public Vector2 currentDirection = Vector2.up;

        public Dictionary<HeroManager, Tile> currentPlayerHero = new Dictionary<HeroManager, Tile>();

        public bool canInput = false;

        public void SetUpPlayer()
        {
            currentPostion = GameManager.instance.gridManager.gridTiles[startPos];

            var hero = Instantiate(GameManager.instance.heroPerfab[Random.Range(0, GameManager.instance.heroPerfab.Count)],
                                   currentPostion.transform.position,
                                   currentPostion.transform.rotation);

            hero.RandomSetUp();
            currentPlayerHero.Add(hero, currentPostion);
            playerTile.transform.position = currentPostion.gameObject.transform.position;
        }

        public void ResetPlayer()
        {
            currentPostion = null;
            lastTileOfPlayer = null;
            currentDirection = Vector2.up;

            if (currentPlayerHero.Count > 0)
            {
                foreach (var hero in currentPlayerHero.Keys)
                {
                    Destroy(hero.gameObject);
                }
            }

            currentPlayerHero.Clear();
        }

        public void MovePlayer(Vector2 input)
        {
            Vector2 newPos = currentPostion.gridPosition + input;
            currentPostion = GameManager.instance.gridManager.gridTiles[newPos];
            playerTile.transform.position = currentPostion.gameObject.transform.position;
        }

        public IEnumerator MoveAllHeroNormal()
        {
            Vector2 nextGridPos = currentPostion.gridPosition;
            Tile nextTile = GameManager.instance.gridManager.GetTileAtGridPosition(nextGridPos);
            yield return MoveAllHero(nextTile);
        }

        public IEnumerator MoveAllHeroWithTile(Tile firstTile)
        {
            yield return MoveAllHero(firstTile);
        }

        public IEnumerator MoveAllHero(Tile nextTile)
        {
            List<HeroManager> heroList = new List<HeroManager>(currentPlayerHero.Keys);

            Dictionary<HeroManager, Tile> newHeroTile = new Dictionary<HeroManager, Tile>();

            lastTileOfPlayer = currentPlayerHero[heroList[heroList.Count - 1]]; //Set For Collect New Hero

            for (int i = 0; i < heroList.Count; i++)
            {
                HeroManager hero = heroList[i];
                Tile oldTile = currentPlayerHero[hero]; //Set For Next Tile
                newHeroTile[hero] = nextTile; //Set Tile
                nextTile = oldTile;//Set For Next Hero
            }

            currentPlayerHero.Clear();

            foreach (var hero in newHeroTile)
            {
                currentPlayerHero[hero.Key] = hero.Value;

                Vector2 targetGridPos = hero.Value.gridPosition;

                hero.Key.StartCoroutine(hero.Key.MoveHero(targetGridPos));
            }

            AudioManager.instance.PlayOneShotSFX(moveSoundKey);

            foreach (HeroManager hero in newHeroTile.Keys)
            {
                yield return new WaitUntil(() => !hero.IsMoving);
            }
        }

        public Dictionary<Vector2, CharacterManager> GetHeroPositions()
        {
            var heroPos = new Dictionary<Vector2, CharacterManager>();

            foreach (var hero in currentPlayerHero)
            {
                Vector2 pos = hero.Value.gridPosition;

                if (!heroPos.ContainsKey(pos))
                {
                    heroPos.Add(pos, hero.Key);
                }
            }

            return heroPos;
        }

        public void CollectedHero(CharacterManager data)
        {
            var character = Instantiate(GetHeroPrefab(data.characterClass), lastTileOfPlayer.transform.position, lastTileOfPlayer.transform.rotation);
            currentPlayerHero.Add(character, lastTileOfPlayer);
            character.GetDataSetUp(data.statusCharacter.GetDataSetup());

            AudioManager.instance.PlayOneShotSFX(collectSoundKey);
        }

        private HeroManager GetHeroPrefab(CharacterClass classed)
        {
            switch (classed)
            {
                case CharacterClass.Paper:
                    return paperPrefab;
                case CharacterClass.Rock:
                    return rockPrefab;
                case CharacterClass.Scissor:
                    return scissorPrefab;
            }

            return null;
        }

        public void DeletePlayerHeroHasDead()
        {
            var hero = TryGetFirstHero();
            currentPlayerHero.Remove(hero);
            Destroy(hero.gameObject);
        }

        public HeroManager TryGetFirstHero()
        {
            return currentPlayerHero.Keys.First();
        }

        public int CheckHeroRemaining()
        {
            return currentPlayerHero.Count;
        }

        public void SwapHeroForward()
        {
            SwapHero(true);
        }

        public void SwapHeroBackward()
        {
            SwapHero(false);
        }

        private void SwapHero(bool isForward)
        {
            if (currentPlayerHero.Count <= 1 || !canInput) return;

            var heroList = currentPlayerHero.Keys.ToList();
            var tileList = currentPlayerHero.Values.ToList();

            if (isForward)
            {
                var firstHero = heroList[0];
                heroList.RemoveAt(0);
                heroList.Add(firstHero);
            }
            else
            {
                var lastHero = heroList[heroList.Count - 1];
                heroList.RemoveAt(heroList.Count - 1);
                heroList.Insert(0, lastHero);
            }

            var newDict = new Dictionary<HeroManager, Tile>();

            for (int i = 0; i < heroList.Count; i++)
            {
                var data = heroList[i].statusCharacter.GetDataSetup();
                heroList[i] = Instantiate(GetHeroPrefab(heroList[i].characterClass), tileList[i].transform.position, tileList[i].transform.rotation);
                heroList[i].GetDataSetUp(data);
                heroList[i].GetDataDirection(currentDirection);
                newDict[heroList[i]] = tileList[i];
            }

            foreach (var hero in currentPlayerHero.Keys)
            {
                Destroy(hero.gameObject);
            }

            currentPlayerHero.Clear();
            currentPlayerHero = newDict;

            GameManager.instance.gameplayUIManager.RemovePlayerProfile();
            GameManager.instance.gameplayUIManager.SetUpPlayerGameplayUI(TryGetFirstHero().statusCharacter);
        }
    }
}