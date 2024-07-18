using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

[RequireComponent(typeof(Button))]
public class ProgressGame : MonoBehaviour
{
    private Button button;
    private bool firstButtonPressed = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Progress);
    }

    private void Progress()
    {
        ServerRpcParams serverRpcParams = default;
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.onPlayerReadyCheck.Invoke(serverRpcParams);

        if (firstButtonPressed) return;
        gameManager.onGameStateChange(GameState.Tutorial);
        FindFirstObjectByType<LobbyManager>().onUITypeChange(TypeOfUIWindow.TutorialMenu);
        firstButtonPressed = true;

    }


}
