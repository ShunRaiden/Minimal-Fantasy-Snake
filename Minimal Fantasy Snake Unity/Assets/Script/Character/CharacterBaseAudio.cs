using UnityEngine;

public class CharacterBaseAudio : MonoBehaviour
{
    public string attackSoundKey;
    public string deadSoundKey;
    public string takeDamageSoundKey;

    public void PlaySound(string targetSound)
    {
        //AudioManager.Instance.PlaySound(attackSoundKey);
    }
}
