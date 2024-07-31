using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AfterCreation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waitingTextMesh;
    [SerializeField] private Button playButton;
    private float timer = 0;
    private int index = 0;
    private string waitingText;
    private bool enableTimer = false;

    private void Start()
    {
        waitingText = waitingTextMesh.text;
        playButton.onClick.AddListener(StartStory);

        FindFirstObjectByType<LobbyManager>().OnLobbyCreationAddListener(SetTimer);

        NetworkManager.Singleton.OnClientConnectedCallback += ClientHasJoined;
    }

    private void FixedUpdate()
    {
        if (!enableTimer) return;
        timer += Time.fixedDeltaTime;
        if(timer > 1)
        {
            timer = 0;
            index++;
            waitingTextMesh.text += ".";
            if (index > 3)
            {
                waitingTextMesh.text = waitingText;
                index = 0;
            }
        }
    }

    private void SetTimer(string placeHolder)
    {
        playButton.gameObject.SetActive(false);
        enableTimer = true;
        waitingTextMesh.text = waitingText;
    }

    private void ClientHasJoined(ulong clientId)
    {
        enableTimer = false;
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            waitingTextMesh.text = "Your partner connected!";
            playButton.gameObject.SetActive(true);
        }
        else
        {
            if (NetworkManager.Singleton.IsHost) return;
            waitingTextMesh.text = "Connected successfully!";
        }
    }

    private void StartStory()
    {
        FindFirstObjectByType<GameManager>().SetGameStateServerRpc(GameState.Story);
    }
}
