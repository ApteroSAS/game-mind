using UnityEngine;


public enum TypeOfUIWindow
{
    MainMenu,
    LobbyMenu,
    CreditsMenu,
    StoryMenu,
    TutorialMenu,
    InGameMenu,
}

[RequireComponent(typeof(CanvasGroup))]
public class UIType : MonoBehaviour
{
    [SerializeField] private TypeOfUIWindow typeOfUIWindow;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        FindFirstObjectByType<UIManager>().OnUITypeChangeAddListener(ToggleCanvasGroup);
    }

    private void ToggleCanvasGroup(TypeOfUIWindow newTypeOfUIWindow)
    {
        canvasGroup.ToggleCanvasGroup(newTypeOfUIWindow == typeOfUIWindow);
    }



}
