using Manager;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIGameplaysPanel : MonoBehaviour
    {
        public event Action<int> OnGameSpeedButtonClickEvent;

        [SerializeField] TMP_Text currentSpeedText;
        [SerializeField] Button returnMainMenuButton;
        [SerializeField] List<UIPlayRateSlot> uIPlayRateSlot = new List<UIPlayRateSlot>();

        private void Awake()
        {
            foreach (var slot in uIPlayRateSlot)
            {
                slot.SetUp();
                slot.Init();
                slot.OnGameSpeedButtonClickEvent += OnSpeedButtonClick;
            }

            returnMainMenuButton.onClick.AddListener(OnReturnToMainMenu);
        }

        private void OnDestroy()
        {
            foreach (var slot in uIPlayRateSlot)
            {
                slot.Dispose();
                slot.OnGameSpeedButtonClickEvent -= OnSpeedButtonClick;
            }

            returnMainMenuButton.onClick.RemoveAllListeners();
        }

        public void OnSpeedButtonClick(int speed)
        {
            OnGameSpeedButtonClickEvent?.Invoke(speed);
        }

        public void UpdateGameSpeedText()
        {
            currentSpeedText.text = $"Current Speed : x{GameManager.instance.TryGetGameSpeedInt()}";
        }

        public void OnReturnToMainMenu()
        {
            GameManager.instance.SetUpMainMenu();
        }
    }
}