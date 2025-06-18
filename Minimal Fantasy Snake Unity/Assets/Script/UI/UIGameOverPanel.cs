using Manager;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIGameOverPanel : MonoBehaviour
    {
        public event Action OnRestartButtonClickEvent;
        public event Action OnMianMenuButtonClickEvent;

        [SerializeField] GameObject content;

        [SerializeField] Button restartButton;
        [SerializeField] Button mianMenuButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
            mianMenuButton.onClick.AddListener(OnMianMenuButtonClick);

            GameManager.instance.OnGameOverEvent += OpenPanel;
            GameManager.instance.OnStartGameEvent += ClosePanel;
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
            mianMenuButton.onClick.RemoveAllListeners();

            GameManager.instance.OnGameOverEvent -= OpenPanel;
            GameManager.instance.OnStartGameEvent -= ClosePanel;
        }

        public void OpenPanel()
        {
            content.SetActive(true);
        }

        public void ClosePanel()
        {
            content.SetActive(false);
        }

        public void OnRestartButtonClick()
        {
            OnRestartButtonClickEvent?.Invoke();
        }

        public void OnMianMenuButtonClick()
        {
            OnMianMenuButtonClickEvent?.Invoke();
        }
    }
}