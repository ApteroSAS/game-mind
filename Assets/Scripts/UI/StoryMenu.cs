using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

public class StoryMenu : MonoBehaviour
{
    [SerializeField] private Camera lobbyCamera;

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

    private int index = 0;

    private bool isActive = false;
    private float alpha = 1;
    private bool ascending = false;

    public delegate void OnTextScroll();
    private OnTextScroll onTextScroll;

    public void OnTextScrollAddListener(OnTextScroll listener)
    {
        onTextScroll += listener;
    }

    public void OnTextScrollInvoke()
    {
        onTextScroll.Invoke();
    }

    private void Awake()
    {
        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(PlayVideo);
    }

    private void Update()
    {
        if (isActive)
        {
            if (ascending) alpha += 2f * Time.deltaTime;
            else alpha -= 2f * Time.deltaTime;

            if (alpha <= 0) ascending = true;
            if (alpha >= 1) ascending = false;

            arrow.alpha = alpha;

            if(Input.GetMouseButtonDown(0))
            LoadNextText();
        }
    }

    private void LoadNextText()
    {
        OnTextScrollInvoke();
        if (index >= 3) 
        {
            LoadNextScene();
            return;
        }
        index++;
        textMesh.text = paragraphs[index];
    }

    private void LoadNextScene()
    {
        isActive = false;
        storyCamera.gameObject.SetActive(false);
        lobbyCamera.gameObject.SetActive(false);

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>().EnableCameraAndMovement(true);

        UIManager uiManager = FindFirstObjectByType<UIManager>();

        uiManager.OnUITypeChangeInvoke(TypeOfUIWindow.TutorialMenu);
        uiManager.OnUITypeChangeInvoke(TypeOfUIWindow.InGameMenu);
    }

    private void PlayVideo(GameState gameState)
    {
        if (gameState == GameState.Story)
        {
            StartCoroutine(PlayVideoAsync());
        }
    }

    private void ResetScene()
    {
        isActive = true;
        index = 0;
        textMesh.text = paragraphs[index];
    }

    IEnumerator PlayVideoAsync()
    {
        ResetScene();

        string proxy = "https://cors-anywhere.herokuapp.com/";
        string url = "https://meet.aptero.co/files/public/Video/MIND-INTRO-video-low.mp4";

        storyVideo.url = proxy + url;
        storyVideo.Prepare();

        while (!storyVideo.isPrepared)
        {
            yield return null;
        }

        lobbyCamera.gameObject.SetActive(false);
        storyCamera.gameObject.SetActive(true);
        storyVideo.Play();
    }

}
