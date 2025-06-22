using TMPro;
using UnityEngine;

namespace UI
{
    public class UIDamageNumberSlot : MonoBehaviour
    {
        [SerializeField] TMP_Text damageNumber;

        public void SpawnDamageNumber(string amount)
        {
            damageNumber.text = $"- {amount}";
            Destroy(gameObject, 1f);
        }
    }
}