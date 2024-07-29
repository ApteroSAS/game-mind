using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField] Button returnButton;

    private void Awake()
    {
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();

        returnButton.onClick.AddListener(() => ReturnButton(lobbyManager));
    }

    private void ReturnButton(LobbyManager lobbyManager)
    {
        lobbyManager.OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
    }
}
