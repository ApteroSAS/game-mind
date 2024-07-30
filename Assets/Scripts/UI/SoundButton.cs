using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundButton : MonoBehaviour
{
    [SerializeField] SoundType soundType;

    [SerializeField] private Sprite mutedTexture;
    [SerializeField] private Sprite unmutedTexture;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ToggleMute);
        FindFirstObjectByType<SoundManager>().OnSoundTypeToggleMuteAddListener(ToggleTexture);
    }

    private void ToggleMute()
    {
        FindFirstObjectByType<SoundManager>().ToggleMuteSoundTypeInvoke(soundType);
    }

    private void ToggleTexture(SoundType invokedSoundType, bool isMuted)
    {
        if (invokedSoundType != soundType) return;

        if (isMuted)
        {
            GetComponent<Image>().sprite = mutedTexture;
        }
        else
        {
            GetComponent<Image>().sprite = unmutedTexture;
        }
    }
}
