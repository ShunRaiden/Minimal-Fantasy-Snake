using UnityEngine;

namespace Character
{
    [System.Serializable]
    public class CharacterBaseStatus
    {
        public int MAX_HP = 100;
        public int ATK = 10;
        public int DEF = 5;

        public int currentHealth;
        public bool IsDead => currentHealth <= 0;

        public void SetUp()
        {
            currentHealth = MAX_HP;
        }
    }
}
