using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBaseUI : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text defText;
    public Image classImage;

    public void UpdateHP(int currentHP)
    {
        hpText.text = $"{currentHP}";
    }

    public void SetUpUI(int currentHP, int akt, int def, Sprite classIcon)
    {
        hpText.text = $"{currentHP}";
        atkText.text = $"{akt}";
        defText.text = $"{def}";
        classImage.sprite = classIcon;
    }
}
