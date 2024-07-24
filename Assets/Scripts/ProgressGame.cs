using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

[RequireComponent(typeof(Button))]
public class ProgressGame : MonoBehaviour
{
    private Button button;
    [SerializeField] private GameState[] gameStates;

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

        for (int i = 0; i < gameStates.Length; i++)
        {
            if (gameStates[i] == GameState.Story)
            {
                FindFirstObjectByType<LobbyManager>().onUITypeChange.Invoke(TypeOfUIWindow.TutorialMenu);
            }
        }
    }
}
