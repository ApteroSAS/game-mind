using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public AudioClip clickSound;

    private void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlaySound);
        }

        FindFirstObjectByType<StoryMenu>().OnTextScrollAddListener(PlaySound);
    }

    private void PlaySound()
    {
        GetComponent<AudioSourceExtension>().OnSoundPlayRandomPitch();
    }
}
