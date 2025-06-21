using System.Collections;
using UnityEngine;

namespace Character
{
    public class HeroManager : CharacterManager
    {
        [Header("Hero Base")]
        public bool IsMoving = false;

        public IEnumerator MoveHero(Vector2 targetPosition)
        {
            IsMoving = true;
            movementCharacter.MoveToTarget(targetPosition);
            animationCharacter.SetTriggerAnimation(CharacterBaseAnimation.WALK_ANIM_KEY);
            yield return new WaitUntil(() => movementCharacter.IsFinishDestination());
            animationCharacter.SetTriggerAnimation(CharacterBaseAnimation.IDLE_ANIM_KEY);
            IsMoving = false;
        }
    }
}
