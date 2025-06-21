using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPlayRateSlot : MonoBehaviour
    {
        public event Action<int> OnGameSpeedButtonClickEvent;

        [SerializeField] TMP_Text speedText;
        [SerializeField] Button speedButton;
        [SerializeField] int speedRate;

        public void SetUp()
        {
            speedText.text = $"x{speedRate}";
        }

        public void Init()
        {
            speedButton.onClick.AddListener(() => OnSpeedButtonClick(speedRate));
        }

        public void Dispose()
        {
            speedButton.onClick.RemoveAllListeners();
        }

        public void OnSpeedButtonClick(int speed)
        {
            OnGameSpeedButtonClickEvent?.Invoke(speed);
        }
    }
}
