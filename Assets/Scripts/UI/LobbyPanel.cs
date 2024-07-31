using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup start;
    [SerializeField] CanvasGroup afterCreation;


    private void Awake()
    {
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();

        lobbyManager.OnLobbyCreationAddListener(ChangePanel);
        lobbyManager.OnLobbyJoinAddListener(ChangePanel);
        lobbyManager.OnLobbyLeaveAddListener(ResetPanel);
    }

    private void ChangePanel(string placeHolder)
    {
        start.ToggleCanvasGroup(false);
        afterCreation.ToggleCanvasGroup(true);
    }

    private void ResetPanel()
    {
        start.ToggleCanvasGroup(true);
        afterCreation.ToggleCanvasGroup(false);
    }


}
