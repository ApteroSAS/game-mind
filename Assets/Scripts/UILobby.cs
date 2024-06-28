using UnityEngine;

public class UILobby : MonoBehaviour
{
    [SerializeField] private LobbyType[] lobbyTypes;
    private LobbyType currentLobbyType;

    void Start()
    {
        foreach (var type in lobbyTypes)
        {
            if (type.GetComponent<LobbyType>().GetLobbyType() == TypeOfLobbyWindow.LOBBYMENU)
            currentLobbyType = type;
        }
        TurnCanvasGroup(currentLobbyType, true);
    }

    private void Update()
    {
        if (currentLobbyType.GetLobbyType() == TypeOfLobbyWindow.LOBBYMENU) return;

        if (Input.anyKeyDown)
        {
            TurnCanvasGroup(currentLobbyType, false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TurnCanvasGroup(currentLobbyType, true);
        }
    }

    private void TurnCanvasGroup(LobbyType lobbyType, bool turnOn)
    {
        CanvasGroup canvasGroup = lobbyType.GetComponent<CanvasGroup>();
        canvasGroup.alpha = turnOn ? 1 : 0;
        canvasGroup.interactable = turnOn;
        canvasGroup.blocksRaycasts = turnOn;
    }

    public void SetCurrentLobby(TypeOfLobbyWindow typeOfLobbyWindow)
    {
        foreach(var type in lobbyTypes)
        {
            if (type.GetComponent<LobbyType>().GetLobbyType() == typeOfLobbyWindow)
            {
                currentLobbyType = type;
            } else
            {
                TurnCanvasGroup(type, false);
            }
        }
        TurnCanvasGroup(currentLobbyType, true);
    }
}
