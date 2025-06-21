using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput playerInput;
        [SerializeField] PlayerManager playerManager;

        public Vector2 movementInput;

        private void OnEnable()
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();
            }

            playerInput.PlayerMovement.Movement.started += OnMovementStarted;

            playerInput.Enable();
        }

        private void OnDisable()
        {
            if (playerInput != null)
            {
                playerInput.PlayerMovement.Movement.started -= OnMovementStarted;

                playerInput.Disable();
            }
        }

        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();

            var direction = new Vector2(input.x, input.y);

            if (!OnCheckCanMove(direction)) return;

            playerManager.MovePlayer(direction);
            playerManager.currentDirection = direction;
            movementInput = Vector2.zero;
            playerManager.canInput = false;
        }

        private bool OnCheckCanMove(Vector2 dir)
        {
            if (!playerManager.canInput)
            {
                return false;
            }

            // Protect walking to the opposite direction
            if (dir + playerManager.currentDirection == Vector2.zero)
            {
                return false;
            }

            Vector2 targetPos = playerManager.currentPostion.GridPosition + dir;

            // Check is still in Grid
            if (!GameManager.instance.gridManager.IsPositionInGrid(targetPos))
            {
                return false;
            }

            return true;
        }
    }
}