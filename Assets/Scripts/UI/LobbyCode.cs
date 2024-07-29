using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyCode : MonoBehaviour
{
    private TMP_InputField lobbyCodeText;
    //private Button copyButton;

    private string code;

    private void Awake()
    {
        lobbyCodeText = GetComponent<TMP_InputField>();
        //copyButton = GetComponentInChildren<Button>();

        //copyButton.onClick.AddListener(CopyToClipboard);

        FindFirstObjectByType<LobbyManager>().OnLobbyCreationAddListener(UpdateCode);
    }

    private void UpdateCode(string lobbyCode)
    {
        code = lobbyCode;
        lobbyCodeText.text = code;
    }

    void CopyToClipboard()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //
#else
        GUIUtility.systemCopyBuffer = code;
#endif
    }
}
