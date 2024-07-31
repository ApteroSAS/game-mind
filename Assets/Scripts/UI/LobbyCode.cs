using UnityEngine;
using TMPro;

public class LobbyCode : MonoBehaviour
{
    private TMP_InputField lobbyCodeText;

    private string code;

    private void Awake()
    {
        lobbyCodeText = GetComponent<TMP_InputField>();

        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        lobbyManager.OnLobbyCreationAddListener(UpdateCode);
        lobbyManager.OnLobbyJoinAddListener(UpdateCode);
    }

    private void UpdateCode(string lobbyCode)
    {
        code = lobbyCode;
        lobbyCodeText.text = code;
    }

}
