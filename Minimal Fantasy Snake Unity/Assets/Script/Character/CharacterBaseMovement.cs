using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class CharacterBaseMovement : MonoBehaviour
    {
        NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public bool IsFinishDestination()
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveToTarget(Vector2 targetPosition)
        {
            Vector3 position = new Vector3(targetPosition.x, 0, targetPosition.y);
            agent.SetDestination(position);
            RatationDirection(position);
        }
        public void RatationDirection(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = lookRotation;
        }
    }
}
