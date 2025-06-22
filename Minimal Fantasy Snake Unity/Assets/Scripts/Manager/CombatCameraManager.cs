using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class CombatCameraManager : MonoBehaviour
    {
        [SerializeField] GameObject upCamera;
        [SerializeField] GameObject downCamera;
        [SerializeField] GameObject leftCamera;
        [SerializeField] GameObject rightCamera;

        private GameObject currentCamera;

        [SerializeField] Animator fadeAnim;

        private Vector2 upDirection = new Vector2(0, 1);
        private Vector2 downDirection = new Vector2(0, -1);
        private Vector2 rightDirection = new Vector2(1, 0);
        private Vector2 leftDirection = new Vector2(-1, 0);

        Dictionary<Vector2, GameObject> allDataCameraPosition;

        void Awake()
        {
            allDataCameraPosition = new Dictionary<Vector2, GameObject>{{ upDirection, upCamera },
                                                                        { downDirection, downCamera },
                                                                        { leftDirection, leftCamera },
                                                                        { rightDirection, rightCamera }};
        }

        public IEnumerator StartCombatCamera()
        {
            fadeAnim.Play("Fade");
            yield return new WaitForSeconds(0.5f);
            SetUpPostion();
            yield return new WaitForSeconds(0.5f);
        }

        public IEnumerator StopCombatCamera()
        {
            fadeAnim.Play("Fade");
            yield return new WaitForSeconds(0.5f);
            currentCamera.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

        private void SetUpPostion()
        {
            Vector2 targetDir = GameManager.instance.playerManager.currentDirection;

            if (allDataCameraPosition.ContainsKey(targetDir))
            {
                currentCamera = allDataCameraPosition[targetDir];
            }

            currentCamera?.SetActive(true);
        }
    }
}