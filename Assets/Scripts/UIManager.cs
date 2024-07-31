using UnityEngine;
using static LobbyManager;

public class UIManager : MonoBehaviour
{
    int index = 0;

    public delegate void OnUITypeChange(TypeOfUIWindow typeOfUIWindow);
    private OnUITypeChange onUITypeChange;

    public void OnUITypeChangeAddListener(OnUITypeChange listener)
    {
        onUITypeChange += listener;
    }

    public void OnUITypeChangeInvoke(TypeOfUIWindow typeOfUIWindow)
    {
        onUITypeChange.Invoke(typeOfUIWindow);
    }


    private void Awake()
    {
        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(InvokeUIBasedOnGameState);
    }

    private void InvokeUIBasedOnGameState(GameState gameState)
    {
        index++;
        Debug.Log("I got invoked a total amount of: " + index);
        switch (gameState)
        {
            case GameState.Menu:
                OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
                break;
            case GameState.Story:
                OnUITypeChangeInvoke(TypeOfUIWindow.StoryMenu);
                break;

            default:
                OnUITypeChangeInvoke(TypeOfUIWindow.InGameMenu);
                break;
        }
    }
}
