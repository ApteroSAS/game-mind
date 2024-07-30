using UnityEngine;
using UnityEngine.UI;

public class UIResults : MonoBehaviour
{
    [SerializeField] private CanvasGroup buttonPanel;
    [SerializeField] private CanvasGroup endingPanel;
    [SerializeField] private Button[] buttons;

    private bool isVisible = false;

    private void Awake()
    {
        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(ToggleButtonPanel);
        FindFirstObjectByType<LobbyManager>().OnLobbyLeaveAddListener(ToggleBothPanelsOff);

        foreach (var item in buttons)
        {
            item.onClick.AddListener(ToggleEndingPanel);
        }
    }

    private void ToggleButtonPanel(GameState gameState)
    {
        if (gameState == GameState.Result)
        {
            buttonPanel.ToggleCanvasGroup(true);
        }
        else
        {
            buttonPanel.ToggleCanvasGroup(false);
        }
    }

    private void ToggleBothPanelsOff()
    {
        buttonPanel.ToggleCanvasGroup(false);
        endingPanel.ToggleCanvasGroup(false);
        isVisible = false;
    }

    private void ToggleEndingPanel()
    {
        isVisible = !isVisible;

        endingPanel.ToggleCanvasGroup(isVisible);
    }
}
