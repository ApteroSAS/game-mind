using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CanvasGroup))]
public class ProgressGame : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Sprite readyTexture;
    [SerializeField] private Sprite unReadyTexture;
    [SerializeField] private TMP_FontAsset readyFont;
    [SerializeField] private TMP_FontAsset unReadyFont;
    [SerializeField] private Image image;

    private void Awake()
    {
        button.onClick.AddListener(Progress);

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.OnGameStateChangeAddListener(ButtonInteractableBasedOnGameState);
        gameManager.OnPlayerReadyReceiveAddListener(UpdateButton);
        
    }

    public static void Progress()
    {
        ServerRpcParams serverRpcParams = default;

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.OnPlayerReadyInvoke(serverRpcParams); 
    }

    private void UpdateButton(ResponsibleFor responsibleFor, bool isReady)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if(responsibleFor == ResponsibleFor.Host)
            {
                UpdateButtonTexture(isReady);
            }
        }
        else
        {
            if(responsibleFor == ResponsibleFor.Guest)
            {
                UpdateButtonTexture(isReady);
            }
        }
    }

    private void UpdateButtonTexture(bool isReady)
    {
        if (!isReady)
        {
            textMesh.text = "Ready!";
            textMesh.font = readyFont;
            image.sprite = readyTexture;
        }
        else
        {
            textMesh.text = "Not ready";
            textMesh.font = unReadyFont;
            image.sprite = unReadyTexture;
        }
    }

    private void ButtonInteractableBasedOnGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Question2:
                canvasGroup.ToggleCanvasGroup(true);
                break;
            case GameState.Question3:
                canvasGroup.ToggleCanvasGroup(true);
                break;
            case GameState.Question4:
                canvasGroup.ToggleCanvasGroup(true);
                break;

            default:
                canvasGroup.ToggleCanvasGroup(false);
                break;
        }
    }
}
