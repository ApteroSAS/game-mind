using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Button soundButton;
    [SerializeField] Button musicButton;
    [SerializeField] Button leaveLobbyButton;

    private void Awake()
    {
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        leaveLobbyButton.onClick.AddListener(lobbyManager.LeaveLobby);
    }


}
