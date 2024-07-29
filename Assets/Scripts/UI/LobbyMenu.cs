using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_InputField inputLobbyCode;

    private void Awake()
    {
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();

        createLobbyButton.onClick.AddListener(lobbyManager.CreateLobby);
        joinLobbyButton.onClick.AddListener(() => lobbyManager.JoinLobby(inputLobbyCode.text));
        returnButton.onClick.AddListener(() => ReturnButton(lobbyManager));
        returnButton.onClick.AddListener(lobbyManager.LeaveLobby);
    }

    private void ReturnButton(LobbyManager lobbyManager)
    {
        lobbyManager.OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
    }



}
