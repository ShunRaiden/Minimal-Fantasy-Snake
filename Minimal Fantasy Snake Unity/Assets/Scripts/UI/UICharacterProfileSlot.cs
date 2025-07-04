using Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UICharacterProfileSlot : MonoBehaviour
    {
        Animator animator;

        [SerializeField] TMP_Text hp_Text;
        [SerializeField] TMP_Text atk_Text;
        [SerializeField] TMP_Text def_Text;

        [SerializeField] Image profileImage;
        [SerializeField] Image classImage;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetUpSlot(CharacterBaseStatus data)
        {
            UpdateDataSlot(data);
            profileImage.sprite = data.IconSprite;
            classImage.sprite = data.IconClass;
        }

        public void UpdateDataSlot(CharacterBaseStatus data)
        {
            hp_Text.text = data.currentHealth.ToString();
            atk_Text.text = data.currentATK.ToString();
            def_Text.text = data.currentDEF.ToString();
        }

        public void RemoveSlot()
        {
            PlayAnimation("Remove");
            Destroy(gameObject, 2f);
        }

        public void PlayAnimation(string anim)
        {
            animator.Play(anim);
        }
    }
}
