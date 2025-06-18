using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public class CharacterBaseAnimation : MonoBehaviour
    {
        private Animator animator;

        public const string IDLE_ANIM_KEY = "Idle";
        public const string WALK_ANIM_KEY = "Walking";
        public const string ATTACK_ANIM_KEY = "Attack";
        public const string TAKE_DAMAGE_KEY = "TakeDamage";
        public const string DEAD_ANIM_KEY = "Dead";

        public float attackTiming;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void SetTriggerAnimation(string anim)
        {
            animator.SetTrigger(anim);
        }

        public void PlayTargetAniamtion(string anim)
        {
            animator.Play(anim);
        }
    }
}
