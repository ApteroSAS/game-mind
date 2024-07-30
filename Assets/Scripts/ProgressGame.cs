using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System.Xml;

[RequireComponent(typeof(Button))]
public class ProgressGame : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Sprite readyTexture;
    [SerializeField] private Sprite unReadyTexture;
    [SerializeField] private Image image;

    private void Awake()
    {
        button.onClick.AddListener(Progress);

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.OnGameStateChangeAddListener(ButtonInteractableBasedOnGameState);
        gameManager.OnPlayerReadyReceiveAddListener(UpdateButtonTexture);
        
    }

    public static void Progress()
    {
        ServerRpcParams serverRpcParams = default;

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.OnPlayerReadyInvoke(serverRpcParams); 
    }

    private void UpdateButtonTexture(ResponsibleFor responsibleFor, bool isReady)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if(responsibleFor == ResponsibleFor.Host)
            {
                if (isReady)
                {
                    textMesh.text = "Ready!";
                    image.sprite = readyTexture;
                }
                else
                {
                    textMesh.text = "Not ready";
                    image.sprite = unReadyTexture;
                }
            }
        }
        else
        {
            if(responsibleFor == ResponsibleFor.Guest)
            {
                if (isReady)
                {
                    textMesh.text = "Ready!";
                    image.sprite = readyTexture;
                }
                else
                {
                    textMesh.text = "Not ready";
                    image.sprite = unReadyTexture;
                }
            }
        }
    }

    private void ButtonInteractableBasedOnGameState(GameState gameState)
    {
        switch (gameState)
        {
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
