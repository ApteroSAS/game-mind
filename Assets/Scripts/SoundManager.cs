using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource riverAndBirdsMusic;

    public List<AudioSource> musics = new List<AudioSource>();
    public List<AudioSource> sounds = new List<AudioSource>();

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

    private void OnSoundTypeToggleMuteInvoke(SoundType soundType, bool isMuted)
    {
        onSoundTypeMuteToggle.Invoke(soundType, isMuted);
    }
    #endregion

    private void Awake()
    {
        musics.Add(backgroundMusic);
        musics.Add(riverAndBirdsMusic);

        ToggleMuteSoundTypeAddListener(ToggleMute);
        SetBaseVolume();

        foreach (var item in musics)
        {
            item.Play();
        }
    }

    private void SetBaseVolume()
    {
        foreach (var item in musics)
        {
            item.volume = volumeMusic;
        }

        foreach (var item in sounds)
        {
            item.volume = volumeSound;
        }
    }

    private void ToggleMute(SoundType soundType)
    {
        if(soundType == SoundType.MusicEffect)
        {
            isMusicMuted = !isMusicMuted;
            foreach (var item in musics)
            {
                item.mute = isMusicMuted;
            }
            OnSoundTypeToggleMuteInvoke(soundType, isMusicMuted);
        }
        else
        {
            isSoundMuted = !isSoundMuted;
            foreach (var item in sounds)
            {
                item.mute = isSoundMuted;
            }
            OnSoundTypeToggleMuteInvoke(soundType, isSoundMuted);
        }
    }
}
