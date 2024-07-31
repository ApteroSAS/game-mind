using UnityEngine;
using UnityEngine.UI;

public class Q4_Book : MonoBehaviour, IInteractable
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button button;

    private AudioSourceExtension audioSourceExtension;
    private bool isVisible = false;

    private void Awake()
    {
        audioSourceExtension = GetComponent<AudioSourceExtension>();
        button.onClick.AddListener(Interact);
    }

    public void Interact()
    {
        isVisible = !isVisible;

        canvasGroup.ToggleCanvasGroup(isVisible);
        audioSourceExtension.OnSoundPlayRandomPitch();
    }

    public void StopInteract()
    {
        //
    }
}
