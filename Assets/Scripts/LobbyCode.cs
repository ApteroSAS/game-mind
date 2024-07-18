using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class LobbyCode : MonoBehaviour
{

    private TMP_InputField lobbyCodeText;
    private CanvasGroup canvasGroup;
    private Button copyButton;

    private string code;

    private void Awake()
    {
        lobbyCodeText = GetComponent<TMP_InputField>();
        canvasGroup = GetComponent<CanvasGroup>();
        copyButton = GetComponentInChildren<Button>();

        copyButton.onClick.AddListener(CopyToClipboard);

        FindFirstObjectByType<LobbyManager>().onLobbyCreation += UpdateCode;
        FindFirstObjectByType<GameManager>().onClientJoinLobby += HideCode;
    }

    private void UpdateCode(string lobbyCode)
    {
        code = lobbyCode;
        lobbyCodeText.text = code;
        canvasGroup.ToggleCanvasGroup(true);
    }

    private void HideCode()
    {
        canvasGroup.ToggleCanvasGroup(false);
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
