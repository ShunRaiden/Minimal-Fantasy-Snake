using Manager;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMainMenuPanel : MonoBehaviour
    {
        public event Action OnStartButtonClickEvent;
        public event Action OnQuitButtonClickEvent;

        [SerializeField] GameObject content;

        [SerializeField] Button startButton;
        [SerializeField] Button quitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);

            GameManager.instance.OnStartGameEvent += ClosePanel;
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();

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

        public void OnStartButtonClick()
        {
            OnStartButtonClickEvent?.Invoke();
        }

        public void OnQuitButtonClick()
        {
            OnQuitButtonClickEvent?.Invoke();
        }
    }
}