using UnityEngine;
using UnityEngine.UI;

public class UIReady : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        gameManager.OnGameStateChangeAddListener(ShowCanvasBasedOnGameState);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void ShowCanvasBasedOnGameState(GameState gameState)
    {
        bool visible;
        switch (gameState)
        {
            case GameState.Question1:
                visible = true;
                break;
            case GameState.Question2:
                visible = true;
                break;
            case GameState.Question3:
                visible = true;
                break;
            case GameState.Question4:
                visible = true;
                break;
            default:
                visible = false;
                break;
        }
        canvasGroup.ToggleCanvasGroup(visible);
    }
}
