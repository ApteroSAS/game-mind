using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceExtension : MonoBehaviour
{
    [SerializeField] private SoundType soundType;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (soundType == SoundType.SoundEffect) audioSource.playOnAwake = false;

        SoundManager soundManager = FindFirstObjectByType<SoundManager>();
        soundManager.OnSoundTypeToggleMuteAddListener(ToggleMute);
        soundManager.OnVolumeChangeAddListener(ChangeVolume);

        audioSource.volume = soundManager.GetCurrentVolume(soundType);
    }

    private void ToggleMute(SoundType invokedSoundType, bool isMuted)
    {
        if(soundType == invokedSoundType)
        {
            audioSource.mute = isMuted;
        }
    }

    private void ChangeVolume(SoundType invokedSoundType, float newVolume)
    {
        if(soundType == invokedSoundType)
        {
            audioSource.volume = newVolume;
        }
    }

    public void OnSoundPlayRandomPitch()
    {
        if (soundType == SoundType.MusicEffect) return;

        float randomPitch = Random.Range(0.90f, 1.1f);
        audioSource.pitch = randomPitch;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        SoundManager soundManager = FindFirstObjectByType<SoundManager>();
        soundManager.OnSoundTypeToggleMuteRemoveListener(ToggleMute);
        soundManager.OnVolumeChangeRemoveListener(ChangeVolume);
    }



}
