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

        public void RandomSetUp()
        {
            statusCharacter.RandomSetUp();
            uiCharacter?.SetUpUI(statusCharacter.currentHealth, statusCharacter.currentATK, statusCharacter.currentDEF);
        }

        public void GetDataSetUp(BaseStatusData data)
        {
            statusCharacter.DataSetUp(data);
            uiCharacter?.SetUpUI(statusCharacter.currentHealth, statusCharacter.currentATK, statusCharacter.currentDEF);
        }

        public virtual void TakeDamage(int amount)
        {
            if (amount > statusCharacter.currentDEF)
            {
                statusCharacter.currentHealth -= amount;

                if (statusCharacter.IsDead)
                    Die();
            }

            animationCharacter?.PlayTargetAniamtion(CharacterBaseAnimation.TAKE_DAMAGE_KEY);
            uiCharacter?.UpdateHP(statusCharacter.currentHealth);
            audioCharacter?.PlaySound(audioCharacter.takeDamageSoundKey);
        }

        public virtual void Attack(CharacterManager target)
        {
            RotateToTarget(target.transform.position);
            animationCharacter?.PlayTargetAniamtion(CharacterBaseAnimation.ATTACK_ANIM_KEY);
            audioCharacter?.PlaySound(audioCharacter.attackSoundKey);
            target.TakeDamage(statusCharacter.currentATK);
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