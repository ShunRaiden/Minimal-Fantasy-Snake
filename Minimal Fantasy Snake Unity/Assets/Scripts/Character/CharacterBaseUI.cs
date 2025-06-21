using TMPro;
using UnityEngine;

public class CharacterBaseUI : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text defText;

    public void UpdateHP(int currentHP)
    {
        hpText.text = $"{currentHP}";
    }

    public void SetUpUI(int currentHP, int akt, int def)
    {
        hpText.text = $"{currentHP}";
        atkText.text = $"{akt}";
        defText.text = $"{def}";
    }
}
