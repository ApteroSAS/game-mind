using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InGameMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button continueButton;
    [SerializeField] Button leaveLobbyButton;

    private bool isIngame = false;

    private void Awake()
    {

        continueButton.onClick.AddListener(ToggleMenu);
        leaveLobbyButton.onClick.AddListener(ToggleMenu);
        
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        lobbyManager.onUITypeChange += CheckIngame;
        leaveLobbyButton.onClick.AddListener(lobbyManager.LeaveLobby);
    }

    void Update()
    {
        if (!isIngame) return;
        if (Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();
    }

    private void ToggleMenu()
    {
        bool visible = canvasGroup.alpha == 1;

        canvasGroup.ToggleCanvasGroup(!visible);
    }

    private void CheckIngame(TypeOfUIWindow typeOfUIWindow)
    {
        if (typeOfUIWindow == TypeOfUIWindow.LobbyMenu) isIngame = false;
        else isIngame = true;
    }

}
