using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSourceExtension))]
[RequireComponent(typeof(Button))]
public class ButtonExtension : MonoBehaviour
{
    private AudioClip clickSound;
    private AudioClip hoverSound;
    private AudioSource audioSource;
    private AudioSourceExtension audioSourceExtension;

    private void Awake()
    {
        clickSound = FindFirstObjectByType<ButtonClick>().clickSound;
        hoverSound = FindFirstObjectByType<ButtonHover>().hoverSound;
        audioSource = GetComponent<AudioSource>();
        audioSourceExtension = GetComponent<AudioSourceExtension>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => PlayHoverSound());
        trigger.triggers.Add(entry);
    }

    private void PlayClickSound()
    {
        audioSource.clip = clickSound;
        audioSourceExtension.OnSoundPlayRandomPitch();
    }

    private void PlayHoverSound()
    {
        audioSource.clip = hoverSound;
        audioSourceExtension.OnSoundPlayRandomPitch();
    }





}
