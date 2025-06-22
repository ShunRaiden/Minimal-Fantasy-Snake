using UnityEngine;
using UnityEngine.VFX;

namespace Character
{
    public class CharacterBaseVisualEffect : MonoBehaviour
    {
        [SerializeField] VisualEffect takeDamageVFX;

        public void OnTakeDamageVFX()
        {
            takeDamageVFX.gameObject.SetActive(true);
            OnPlayVFX(takeDamageVFX);
        }

        private void OnPlayVFX(VisualEffect vfx)
        {
            vfx.Stop();
            vfx.Play();
        }
    }
}