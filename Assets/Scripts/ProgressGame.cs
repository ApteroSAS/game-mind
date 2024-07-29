using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

[RequireComponent(typeof(Button))]
public class ProgressGame : MonoBehaviour
{
    private Button button;
    [SerializeField] private Sprite unReadyTexture;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Progress);

        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(ButtonInteractableBasedOnGameState);
    }

    public static void Progress()
    {
        ServerRpcParams serverRpcParams = default;
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.OnPlayerReadyInvoke(serverRpcParams); 
    }

    private void UpdateButtonTexture()
    {
        //change texture
    }

    private void ButtonInteractableBasedOnGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Story:
                button.interactable = true;
                break;

            case GameState.Question2:
                button.interactable = true;
                break;
            case GameState.Question3:
                button.interactable = true;
                break;
            case GameState.Question4:
                button.interactable = true;
                break;

            default:
                button.interactable = false;
                break;
        }
    }
}
