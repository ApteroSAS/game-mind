using UnityEngine;
using UnityEngine.UI;

public class Q3_QuestionBlock : MonoBehaviour, IInteractable
{
    private bool showingInformation = false;
    private CanvasGroup canvasGroup;
    private AudioSourceExtension audioSourceExtension;
    [SerializeField] private Button xButton;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        audioSourceExtension = GetComponentInChildren<AudioSourceExtension>();
        xButton.onClick.AddListener(Interact);
    }

    public void Interact()
    {
        showingInformation = !showingInformation;
        canvasGroup.ToggleCanvasGroup(showingInformation);
        audioSourceExtension.OnSoundPlayRandomPitch();
    }

    public void StopInteract()
    {
        //
    }

}
