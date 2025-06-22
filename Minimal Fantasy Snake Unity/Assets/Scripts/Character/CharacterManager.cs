using Manager;
using System.Collections;
using UnityEngine;

namespace Character
{
    public enum CharacterType { Hero, Monster }
    public enum CharacterClass { Rock, Paper, Scissor, God }
    public class CharacterManager : MonoBehaviour
    {
        [Header("Character Base")]
        public CharacterType characterType;
        public CharacterClass characterClass;
        public CharacterBaseUI uiCharacter;
        public CharacterBaseAudio audioCharacter;
        public CharacterBaseStatus statusCharacter;
        public CharacterBaseMovement movementCharacter;
        public CharacterBaseAnimation animationCharacter;
        public CharacterBaseVisualEffect visualEffectCharacter;
        public bool isDead = false;

        public void RandomSetUp()
        {
            statusCharacter.RandomSetUp();
            uiCharacter?.SetUpUI(statusCharacter.currentHealth, statusCharacter.currentATK, statusCharacter.currentDEF, statusCharacter.IconClass);
        }

        public void GetDataSetUp(BaseStatusData data)
        {
            statusCharacter.DataSetUp(data);
            uiCharacter?.SetUpUI(statusCharacter.currentHealth, statusCharacter.currentATK, statusCharacter.currentDEF, statusCharacter.IconClass);
        }

        public void GetDataDirection(Vector2 targetDir)
        {
            Vector3 direction = new Vector3(targetDir.x, 0f, targetDir.y);

            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = lookRotation;
        }

        public virtual void TakeDamage(int amount, CharacterClass attackerClass)
        {
            var lastDamage = CalculateDamageByClass(amount, attackerClass);
            GameManager.instance.gameplayUIManager.SpawnDamageNumber(lastDamage);

            if (lastDamage > 0)
            {
                statusCharacter.currentHealth -= lastDamage;

                if (statusCharacter.IsDead)
                    Die();
            }

            animationCharacter?.PlayTargetAniamtion(CharacterBaseAnimation.TAKE_DAMAGE_KEY);
            uiCharacter?.UpdateHP(statusCharacter.currentHealth);
            visualEffectCharacter?.OnTakeDamageVFX();
            AudioManager.instance.PlayOneShotSFX(audioCharacter.TakeDamageSoundKey);
        }

        public virtual IEnumerator Attack(CharacterManager target)
        {
            RotateToTarget(target.transform.position);
            yield return StartCoroutine(animationCharacter?.PlayAttackAnimation());
            target.TakeDamage(statusCharacter.currentATK, characterClass);
        }

        protected virtual void Die()
        {
            animationCharacter?.SetTriggerAnimation(CharacterBaseAnimation.DEAD_ANIM_KEY);
            AudioManager.instance.PlayOneShotSFX(audioCharacter.DeadSoundSoundKey);
            isDead = true;
        }

        private void RotateToTarget(Vector3 postion)
        {
            movementCharacter.RatationDirection(postion);
        }

        #region Class Calculated
        private int CalculateDamageByClass(int damageAmount, CharacterClass attacker)
        {
            damageAmount -= statusCharacter.currentDEF;

            if (damageAmount <= 0) return 0;

            if (IsStrongAgainst(attacker, characterClass))
            {
                return damageAmount * 2;
            }
            else if (IsWeakAgainst(attacker, characterClass))
            {
                return Mathf.Max(1, damageAmount / 2);
            }

            return damageAmount;
        }

        private bool IsStrongAgainst(CharacterClass attacker, CharacterClass defender)
        {
            return (attacker == CharacterClass.Rock && defender == CharacterClass.Scissor)
                || (attacker == CharacterClass.Scissor && defender == CharacterClass.Paper)
                || (attacker == CharacterClass.Paper && defender == CharacterClass.Rock);
        }

        private bool IsWeakAgainst(CharacterClass attacker, CharacterClass defender)
        {
            return (attacker == CharacterClass.Rock && defender == CharacterClass.Paper)
                || (attacker == CharacterClass.Scissor && defender == CharacterClass.Rock)
                || (attacker == CharacterClass.Paper && defender == CharacterClass.Scissor);
        }
        #endregion
    }
}