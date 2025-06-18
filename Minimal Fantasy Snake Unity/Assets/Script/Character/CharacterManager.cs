using UnityEngine;

namespace Character
{
    public enum CharacterType { Hero, Monster }
    public class CharacterManager : MonoBehaviour
    {
        [Header("Character Base")]
        public CharacterType characterType;
        public CharacterBaseUI uiCharacter;
        public CharacterBaseAudio audioCharacter;
        public CharacterBaseStatus statusCharacter;
        public CharacterBaseMovement movementCharacter;
        public CharacterBaseAnimation animationCharacter;
        public bool isDead = false;

        private void Awake()
        {
            statusCharacter.SetUp();
            uiCharacter?.SetUpUI(statusCharacter.currentHealth, statusCharacter.MAX_HP, statusCharacter.ATK, statusCharacter.DEF);
        }

        public virtual void TakeDamage(int amount)
        {
            if (amount > statusCharacter.DEF)
            {
                statusCharacter.currentHealth -= amount;

                if (statusCharacter.IsDead)
                    Die();
            }
            uiCharacter?.UpdateHP(statusCharacter.currentHealth, statusCharacter.MAX_HP);
            audioCharacter?.PlaySound(audioCharacter.takeDamageSoundKey);
        }

        public virtual void Attack(CharacterManager target)
        {
            RotateToTarget(target.transform.position);
            animationCharacter?.PlayTargetAniamtion(CharacterBaseAnimation.ATTACK_ANIM_KEY);
            audioCharacter?.PlaySound(audioCharacter.attackSoundKey);
            target.TakeDamage(statusCharacter.ATK);
        }

        protected virtual void Die()
        {
            animationCharacter?.SetTriggerAnimation(CharacterBaseAnimation.DEAD_ANIM_KEY);
            audioCharacter?.PlaySound(audioCharacter.deadSoundKey);
            isDead = true;
        }

        private void RotateToTarget(Vector3 direction)
        {
            movementCharacter.RatationDirection(direction);
        }
    }
}