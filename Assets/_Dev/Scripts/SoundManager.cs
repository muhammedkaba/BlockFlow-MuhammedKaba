using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKDev
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioClip[] sfxClips;

        [SerializeField] private AudioSource pfSfxAudioSource;

        public async UniTaskVoid PlaySound(SFXType type)
        {
            AudioSource audioSource = Instantiate(pfSfxAudioSource);
            audioSource.PlayOneShot(sfxClips[(int)type]);

            await UniTask.Delay(5000);

            if(audioSource) Destroy(audioSource.gameObject);
        }

        public async UniTaskVoid PlaySoundLoop(SFXType type, float seconds)
        {
            AudioSource audioSource = Instantiate(pfSfxAudioSource);
            audioSource.clip = sfxClips[(int)type];
            audioSource.loop = true;
            audioSource.Play();

            await UniTask.Delay((int)(seconds * 1000));

            await StopSoundAsync(audioSource, 0.2f);
        }

        public static async UniTask StopSoundAsync(AudioSource source, float time)
        {
            float timer = time;
            float startVol = source.volume;

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                source.volume = Mathf.Lerp(startVol, 0f, 1f - (timer / time));
                await UniTask.Yield();
            }

            if (source) GameObject.Destroy(source.gameObject);
        }
    }

    public enum SFXType
    {
        BlockUnlocked,
        GrindingBlock,
        BlockSelect,
        GameWon,
        GameLost

    }
}
