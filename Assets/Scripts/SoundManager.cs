using UnityEngine;

public enum SoundType
{
    MusicEffect,
    SoundEffect,
}

public class SoundManager : MonoBehaviour
{
    [SerializeField][Range(0f,1f)] private float volumeMusic = 0.2f;
    [SerializeField][Range(0f,1f)] private float volumeSound = 0.2f;

    private bool isMusicMuted = false;
    private bool isSoundMuted = false;

    #region ToggleMuteSoundType
    public delegate void ToggleMuteSoundType(SoundType soundType);
    private ToggleMuteSoundType toggleMuteSoundType;

    private void ToggleMuteSoundTypeAddListener(ToggleMuteSoundType listener)
    {
        toggleMuteSoundType += listener;
    }

    public void ToggleMuteSoundTypeInvoke(SoundType soundType)
    {
        toggleMuteSoundType.Invoke(soundType);
    }
    #endregion

    #region OnSoundTypeToggleMute
    public delegate void OnSoundTypeToggleMute(SoundType soundType, bool isMuted);
    private OnSoundTypeToggleMute onSoundTypeMuteToggle;

    public void OnSoundTypeToggleMuteAddListener(OnSoundTypeToggleMute listener)
    {
        onSoundTypeMuteToggle += listener;
    }
    public void OnSoundTypeToggleMuteRemoveListener(OnSoundTypeToggleMute listener)
    {
        onSoundTypeMuteToggle -= listener;
    }

    private void OnSoundTypeToggleMuteInvoke(SoundType soundType, bool isMuted)
    {
        onSoundTypeMuteToggle.Invoke(soundType, isMuted);
    }
    #endregion

    #region OnVolumeChange

    public delegate void OnVolumeChange(SoundType soundType, float newVolume);
    private OnVolumeChange onVolumeChange;

    public void OnVolumeChangeAddListener(OnVolumeChange listener)
    {
        onVolumeChange += listener;
    }

    public void OnVolumeChangeRemoveListener(OnVolumeChange listener)
    {
        onVolumeChange -= listener;
    }

    public void OnVolumeChangeInvoke(SoundType soundType, float newVolume)
    {
        onVolumeChange.Invoke(soundType, newVolume);
    }

    #endregion

    private void Awake()
    {
        ToggleMuteSoundTypeAddListener(ToggleMute);
    }


    private void ToggleMute(SoundType soundType)
    {
        if (soundType == SoundType.MusicEffect)
        {
            isMusicMuted = !isMusicMuted;
            OnSoundTypeToggleMuteInvoke(soundType, isMusicMuted);
        }
        else
        {
            isSoundMuted = !isSoundMuted;
            OnSoundTypeToggleMuteInvoke(soundType, isSoundMuted);
        }
    }

    public float GetCurrentVolume(SoundType soundType)
    {
        if (SoundType.MusicEffect == soundType) 
            return volumeMusic;
        else 
            return volumeSound;
    }


}
