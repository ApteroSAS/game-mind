using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StoryMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private CanvasGroup arrow;
    [SerializeField] private Camera storyCamera;
    [SerializeField] private VideoPlayer storyVideo;
    private string[] paragraphs = new string[]
    { "Once upon a time, there was an extraordinary universe known as Mind. Today, you are invited to discover this place.",
        "To triumph, it won't be enough to know the right answer, but to feel the same way as your partner. (Because here, in Mind, it's the symbiosis of your hearts that counts.)",
        "Get ready to explore, laugh and share precious moments. Every corner of this place holds a new question, every nook and cranny an opportunity to strengthen your bond. So take your loved one's hand and let Mind's magic be your guide.",
        "Your adventure begins now. Are you ready to reveal the power of your love ?",
    };

    private bool isActive = false;
    private int index = 0;
    private bool videoEnded = false;

    private float alpha = 1;
    private bool ascending = false;

    private void Awake()
    {
        storyVideo.loopPointReached += OnVideoEnd;

        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(PlayVideo);
    }

    private void Update()
    {
        if (isActive)
        {
            if (ascending) alpha += 0.1f * Time.deltaTime;
            else alpha -= 0.1f * Time.deltaTime;

            if (alpha <= 0) ascending = true;
            if (alpha >= 1) ascending = false;

            arrow.alpha = alpha;

            if(Input.GetMouseButtonDown(0))
            LoadNextText();
        }
    }

    private void LoadNextText()
    {
        if (index >= 3) 
        {
            if (videoEnded)
            {
                isActive = false;
                storyCamera.gameObject.SetActive(false);
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>().EnableCameraAndMovement(true);

                ServerRpcParams serverRpcParams = default;
                FindFirstObjectByType<GameManager>().OnPlayerReadyInvoke(serverRpcParams);
            }
            return;
        }
        index++;
        textMesh.text = paragraphs[index];
    }

    private void PlayVideo(GameState gameState)
    {
        if (gameState == GameState.Story)
        {
            isActive = true;
            Camera.main.gameObject.SetActive(false);
            storyCamera.gameObject.SetActive(true);
            storyVideo.gameObject.SetActive(true);
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoEnded = true;
    }
}
