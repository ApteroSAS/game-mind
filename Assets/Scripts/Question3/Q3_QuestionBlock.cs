using UnityEngine;

public class Q3_QuestionBlock : MonoBehaviour, IInteractable
{
    bool showingInformation = false;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void Interact()
    {
        showingInformation = !showingInformation;

        canvasGroup.alpha = showingInformation ? 1 : 0;
    }

    public void StopInteract()
    {
        //
    }

}
