using Character;
using TMPro;
using UI;
using UnityEngine;

namespace Manager
{
    public class GameplayUIManager : MonoBehaviour
    {
        [Header("CharacterProfile")]
        [SerializeField] Transform playerLayout;
        [SerializeField] Transform monsterLayout;
        [SerializeField] UICharacterProfileSlot uiSlotPrefab;

        [Header("CharacterProfile")]
        [SerializeField] Transform damageNumberLayout;
        [SerializeField] UIDamageNumberSlot uiDamageNumberPerfab;

        [SerializeField] TMP_Text playerCountText;

        [Header("Debug")]
        [SerializeField] UICharacterProfileSlot playerSlot;
        [SerializeField] UICharacterProfileSlot monsterSlot;

        public void UpdatePlayerCount()
        {
            int count = GameManager.instance.playerManager.CheckHeroRemaining();

            if (count > 1)
            {
                playerCountText.text = $"+{count - 1}";
            }
            else
            {
                playerCountText.text = $"";
            }
        }

        public void SetUpPlayerGameplayUI(CharacterBaseStatus character)
        {
            playerSlot = SetUpCharacterGameplaysUI(playerLayout);

            playerSlot?.SetUpSlot(character);
        }

        public void SetUpMonsterGameplayUI(CharacterBaseStatus character)
        {
            monsterSlot = SetUpCharacterGameplaysUI(monsterLayout);

            monsterSlot?.SetUpSlot(character);
        }

        public void UpdatePlayerSlot(CharacterBaseStatus character)
        {
            playerSlot?.UpdateDataSlot(character);
        }

        public void UpdateMonsterSlot(CharacterBaseStatus character)
        {
            monsterSlot?.UpdateDataSlot(character);
        }

        public void PlayPlayerSlotAttackAnimation()
        {
            playerSlot?.PlayAnimation("HeroAttack");
        }

        public void PlayMonsterSlotAttackAnimation()
        {
            monsterSlot?.PlayAnimation("MonsterAttack");
        }

        public void RemovePlayerProfile()
        {
            playerSlot?.RemoveSlot();
            playerSlot = null;
        }

        public void RemoveMonsterProfile()
        {
            monsterSlot?.RemoveSlot();
            monsterSlot = null;
        }

        public void ClearGameplaySlot()
        {
            playerSlot?.RemoveSlot();
            playerSlot = null;

            monsterSlot?.RemoveSlot();
            monsterSlot = null;
        }

        public void SpawnDamageNumber(int amount)
        {
            var dm = Instantiate(uiDamageNumberPerfab, damageNumberLayout);
            dm.SpawnDamageNumber(amount.ToString());
        }

        private UICharacterProfileSlot SetUpCharacterGameplaysUI(Transform layout)
        {
            return Instantiate(uiSlotPrefab, layout);
        }
    }
}