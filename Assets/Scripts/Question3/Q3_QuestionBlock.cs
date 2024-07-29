using UnityEngine;
using UnityEngine.UI;

public class Q3_QuestionBlock : MonoBehaviour, IInteractable
{
    bool showingInformation = false;
    CanvasGroup canvasGroup;
    [SerializeField] Button xButton;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        xButton.onClick.AddListener(Interact);
    }

    public void Interact()
    {
        showingInformation = !showingInformation;

        canvasGroup.ToggleCanvasGroup(showingInformation);
    }

    public void StopInteract()
    {
        //
    }

}
