using UnityEngine;
using UnityEngine.UI;

public enum LobbyPanelUI
{
    Start,
    AfterCreation,
}

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup start;
    [SerializeField] CanvasGroup afterCreation;

    public delegate void OnLobbyPanelChange(LobbyPanelUI lobbyPanelUI);
    private OnLobbyPanelChange onLobbyPanelChange;

    public void OnLobbyPanelChangeAddListener(OnLobbyPanelChange listener)
    {
        onLobbyPanelChange += listener;
    }

    public void OnLobbyPanelChangeInvoke(LobbyPanelUI lobbyPanelUI)
    {
        onLobbyPanelChange.Invoke(lobbyPanelUI);
    }

    private void Awake()
    {
        onLobbyPanelChange += ChangePanel;
    }

    private void ChangePanel(LobbyPanelUI lobbyPanelUI)
    {
        switch (lobbyPanelUI)
        {
            case LobbyPanelUI.Start:
                start.ToggleCanvasGroup(true);
                afterCreation.ToggleCanvasGroup(false);
                break;
            case LobbyPanelUI.AfterCreation:
                start.ToggleCanvasGroup(false);
                afterCreation.ToggleCanvasGroup(true);
                break;
            default:
                break;
        }
    }


}
