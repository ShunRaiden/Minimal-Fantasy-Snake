using TMPro;
using UnityEngine;

public class CharacterBaseUI : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text defText;

    public void UpdateHP(int currentHP, int maxHP)
    {
        hpText.text = $"{currentHP} / {maxHP}";
    }

    public void SetUpUI(int currentHP, int maxHP, int akt, int def)
    {
        hpText.text = $"{currentHP} / {maxHP}";
        atkText.text = $"{akt}";
        defText.text = $"{def}";
    }
}
