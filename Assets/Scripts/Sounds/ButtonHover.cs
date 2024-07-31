using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    public AudioClip hoverSound;

    private void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new()
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((eventData) => PlaySound());
            trigger.triggers.Add(entry);
        }
    }

    private void PlaySound()
    {
        GetComponent<AudioSourceExtension>().OnSoundPlayRandomPitch();
    }

}
