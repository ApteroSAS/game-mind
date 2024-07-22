using UnityEngine;


public enum TypeOfUIWindow
{
    LobbyMenu,
    StoryMenu,
    TutorialMenu,
}

[RequireComponent(typeof(CanvasGroup))]
public class UIType : MonoBehaviour
{
    [SerializeField] private TypeOfUIWindow typeOfUIWindow;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        FindFirstObjectByType<LobbyManager>().onUITypeChange += ToggleCanvasGroup;
    }

    private void ToggleCanvasGroup(TypeOfUIWindow newtypeOfUIWindow)
    {
        canvasGroup.ToggleCanvasGroup(newtypeOfUIWindow == typeOfUIWindow);
    }


}
