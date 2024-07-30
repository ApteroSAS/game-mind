using UnityEngine;
using UnityEngine.UI;

public class Q4_Book : MonoBehaviour, IInteractable
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button button;
    private bool isVisible = false;

    private void Awake()
    {
        button.onClick.AddListener(Interact);
    }

    public void Interact()
    {
        isVisible = !isVisible;
        Debug.Log("canvas should be visible now: " + isVisible);

        canvasGroup.ToggleCanvasGroup(isVisible);
    }

    public void StopInteract()
    {
        //
    }
}
