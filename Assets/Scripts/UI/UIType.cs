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
        FindFirstObjectByType<LobbyManager>().OnUITypeChangeAddListener(ToggleCanvasGroup);
        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(InvokeUIBasedOnGameState);
    }

    private void ToggleCanvasGroup(TypeOfUIWindow newTypeOfUIWindow)
    {
        canvasGroup.ToggleCanvasGroup(newTypeOfUIWindow == typeOfUIWindow);
    }

    private void InvokeUIBasedOnGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                FindFirstObjectByType<LobbyManager>().OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
                break;
            case GameState.Story:
                FindFirstObjectByType<LobbyManager>().OnUITypeChangeInvoke(TypeOfUIWindow.StoryMenu);
                break;
            default:
                break;
        }
    }


}
