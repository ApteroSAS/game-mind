using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIQuestion : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI textMesh;
    CanvasGroup canvasGroup;

    [SerializeField] Image background;
    [SerializeField] TMP_FontAsset[] fontAssets;
    [SerializeField] Sprite[] questionBackground;


    private void Awake()
    {
        gameManager.OnGameStateChangeAddListener(ChangeQuestion);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void ChangeQuestion(GameState gameState)
    {
        string questionText = "";
        bool visible = true;
        switch (gameState)
        {
            case GameState.Question1:
                questionText = "What is your couple's contract?";
                background.sprite = questionBackground[0];
                textMesh.font = fontAssets[0];
                break;
            //case GameState.Question2:
            //    questionText = "What are the 3 fundamental values of a fulfilled couple?"; 
            //    background.sprite = questionBackground[1];
            //    textMesh.font = fontAssets[1];
            //    break;
            case GameState.Question3:
                questionText = "What were the unifying moments in your relationship?"; 
                background.sprite = questionBackground[2];
                textMesh.font = fontAssets[2];
                break;
            case GameState.Question4:
                questionText = "How important is sexuality to you as a couple?"; 
                background.sprite = questionBackground[3];
                textMesh.font = fontAssets[3];
                break;
            default:
                visible = false;
                break;
        }
        textMesh.text = questionText;
        canvasGroup.alpha = visible ? 1 : 0;
    }

}
