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
        public HeroManager heroPrefab;

        public PlayerInputManager inputManager;

        public Vector2 startPos = new Vector2(0, 0);

        public Tile currentPostion;
        public Tile lastTileOfPlayer;
        public Vector2 currentDirection = Vector2.up;

        public Dictionary<HeroManager, Tile> currentPlayerHero = new Dictionary<HeroManager, Tile>();

        public GameObject playerTile;

        public bool canInput = false;

        public string collectHeroSoundKey = "CollectHero";
        public string moveSoundKey = "Move";

        public void SetUpPlayer()
        {
            currentPostion = GameManager.instance.gridManager.gridTiles[startPos];

            var hero = Instantiate(heroPrefab, currentPostion.transform.position, currentPostion.transform.rotation);
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
            Vector2 newPos = currentPostion.GridPosition + input;
            currentPostion = GameManager.instance.gridManager.gridTiles[newPos];
            playerTile.transform.position = currentPostion.gameObject.transform.position;
        }

        public IEnumerator MoveAllHeroNormal()
        {
            Vector2 nextGridPos = currentPostion.GridPosition;
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

                Vector2 targetGridPos = hero.Value.GridPosition;

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
                Vector2 pos = hero.Value.GridPosition;

                if (!heroPos.ContainsKey(pos))
                {
                    heroPos.Add(pos, hero.Key);
                }
            }

            return heroPos;
        }

        public void CollectedHero(BaseStatusData data)
        {
            var character = Instantiate(heroPrefab, lastTileOfPlayer.transform.position, lastTileOfPlayer.transform.rotation);
            currentPlayerHero.Add(character, lastTileOfPlayer);
            character.GetDataSetUp(data);

            AudioManager.instance.PlayOneShotSFX(collectHeroSoundKey);
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
    }
}