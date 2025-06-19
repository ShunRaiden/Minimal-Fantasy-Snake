using Manager;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIGameplaysPanel : MonoBehaviour
    {
        public event Action<int> OnGameSpeedButtonClickEvent;

        [SerializeField] TMP_Text currentSpeedText;
        [SerializeField] List<UIPlayRateSlot> uIPlayRateSlot = new List<UIPlayRateSlot>();

        private void Awake()
        {
            foreach (var slot in uIPlayRateSlot)
            {
                slot.SetUp();
                slot.Init();
                slot.OnGameSpeedButtonClickEvent += OnSpeedButtonClick;
            }
        }

        private void OnDestroy()
        {
            foreach (var slot in uIPlayRateSlot)
            {
                slot.Dispose();
                slot.OnGameSpeedButtonClickEvent -= OnSpeedButtonClick;
            }
        }

        public void OnSpeedButtonClick(int speed)
        {           
            OnGameSpeedButtonClickEvent?.Invoke(speed);
        }

        public void UpdateGameSpeedText()
        {
            currentSpeedText.text = $"Current Speed : x{GameManager.instance.TryGetGameSpeedInt()}";
        }
    }
}