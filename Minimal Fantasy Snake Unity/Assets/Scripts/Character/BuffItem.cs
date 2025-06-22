using UnityEngine;

namespace Character
{
    public enum BuffType { Heal, ATK, DEF }
    public class BuffItem : MonoBehaviour
    {
        [SerializeField] BuffType buffType;

        [SerializeField] float maxBuffStat;
        [SerializeField] float minBuffStat;

        [Header("Debug")]
        [SerializeField] int buffStat;

        private void Awake()
        {
            buffStat = (int)Random.Range(minBuffStat, maxBuffStat);
        }

        public void GetBuff(CharacterManager character)
        {
            character.GetDataSetUp(GetStatusData(character.statusCharacter));
            Destroy(gameObject);
        }

        public BaseStatusData GetStatusData(CharacterBaseStatus character)
        {
            var data = character.GetDataSetup();

            switch (buffType)
            {
                case BuffType.Heal:
                    return new BaseStatusData(data.currentHealth + buffStat, data.currentATK, data.currentDEF);
                case BuffType.ATK:
                    return new BaseStatusData(data.currentHealth, data.currentATK + buffStat, data.currentDEF);
                case BuffType.DEF:
                    return new BaseStatusData(data.currentHealth, data.currentATK, data.currentDEF + buffStat);
                default:
                    return data;
            }
        }
    }
}