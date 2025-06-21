using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager instance { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            if (instance == null)
            {
                instance = this;
            }
        }
        #endregion

        [Header("Audio Source")]
        [SerializeField] AudioSource m_AudioSource;
        [SerializeField] AudioSource s_AudioSource;

        [Header("Base Audio")]
        public const string BASE_BGM = "Base-BGM";
        public const string GAME_LOSE = "Game-Lose";

        private AsyncOperationHandle<AudioClip> bgmHandle;

        public async void PlayMusic(string clipName)
        {
            if (m_AudioSource.isPlaying)
            {
                Addressables.Release(bgmHandle);
            }
            else
            {
                bgmHandle = Addressables.LoadAssetAsync<AudioClip>(clipName);

                await bgmHandle.Task;

                if (bgmHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    m_AudioSource.clip = bgmHandle.Result;
                    m_AudioSource.Play();
                }
                else
                {
                    Debug.LogError($"Cant Find {clipName}");
                }
            }
        }

        public async void PlayOneShotSFX(string clipName)
        {
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(clipName);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = handle.Result;
                s_AudioSource.PlayOneShot(clip);

                await Task.Delay(Mathf.CeilToInt(clip.length * 1000));
                Addressables.Release(handle);
            }
            else
            {
                Debug.LogError($"Cant Find {clipName}");
            }
        }

        public void StopMusic()
        {
            m_AudioSource.Stop();
        }
    }
}