using UnityEngine;

public class Q3_QuestionBlock : Interactable
{
    bool showingInformation = false;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public override void Interact()
    {
        showingInformation = !showingInformation;

        canvasGroup.alpha = showingInformation ? 1 : 0;
    }

    public override void StopInteract()
    {
        //
    }

}
