using UnityEngine;
using TMPro;

public class UIQuestion : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI textMesh;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        gameManager.onGameStateChange += ChangeText;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void ChangeText(GameState gameState)
    {
        string questionText = "";
        bool visible = true;
        switch (gameState)
        {
            case GameState.Question1:
                questionText = "What is your couple's contract?";
                break;
            case GameState.Question2:
                questionText = "What are the 3 fundamental values of a fulfilled couple?";
                break;
            case GameState.Question3:
                questionText = "What were the unifying moments in your relationship?";
                break;
            case GameState.Question4:
                questionText = "How important is sexuality to you as a couple?";
                break;
            default:
                visible = false;
                break;
        }
        textMesh.text = questionText;
        canvasGroup.alpha = visible ? 1 : 0;
    }

}
