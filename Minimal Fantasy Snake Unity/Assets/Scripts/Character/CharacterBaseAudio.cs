using Manager;
using UnityEngine;

namespace Character
{
    [System.Serializable]
    public class CharacterBaseAudio
    {       
        [SerializeField] string takeDamageSoundKey = "TakeDamage";
        public string TakeDamageSoundKey => takeDamageSoundKey;

        [SerializeField] string deadSoundKey = "Dead";
        public string DeadSoundSoundKey => deadSoundKey;
    }
}