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
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();

        createLobbyButton.onClick.AddListener(lobbyManager.CreateLobby);

        joinLobbyButton.onClick.AddListener(() => lobbyManager.JoinLobby(inputLobbyCode.text));

        returnButton.onClick.AddListener(lobbyManager.LeaveLobby);
        returnButton.onClick.AddListener(() => ReturnButton(uiManager));
    }

    private void ReturnButton(UIManager uiManager)
    {
        uiManager.OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
    }



}
