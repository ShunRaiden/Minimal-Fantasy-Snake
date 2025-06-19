using System.Data;
using UnityEngine;

namespace Character
{
    [System.Serializable]
    public class CharacterBaseStatus
    {
        public Sprite iconSprite;
        public Sprite IconSprite => iconSprite;

        [SerializeField] int MAX_HP = 100;
        [SerializeField] int MIN_HP = 50;
        [SerializeField] int MAX_ATK = 10;
        [SerializeField] int MIN_ATK = 5;
        [SerializeField] int MAX_DEF = 4;
        [SerializeField] int MIN_DEF = 1;


        public int currentDEF;
        public int currentATK;
        public int currentHealth;
        public bool IsDead => currentHealth <= 0;

        public void RandomSetUp()
        {
            currentHealth = Random.Range(MIN_HP, MAX_HP + 1);
            currentATK = Random.Range(MIN_ATK, MAX_ATK + 1);
            currentDEF = Random.Range(MIN_DEF, MAX_DEF + 1);
        }

        public void DataSetUp(BaseStatusData data)
        {
            currentHealth = data.currentHealth;
            currentATK = data.currentATK;
            currentDEF = data.currentDEF;
        }

        public BaseStatusData GetDataSetup()
        {
            return new BaseStatusData(currentHealth, currentATK, currentDEF);
        }
    }

    public class BaseStatusData
    {
        public int currentDEF;
        public int currentATK;
        public int currentHealth;

        public BaseStatusData(int hp, int atk, int def)
        {
            currentHealth = hp;
            currentATK = atk;
            currentDEF = def;
        }
    }
}
