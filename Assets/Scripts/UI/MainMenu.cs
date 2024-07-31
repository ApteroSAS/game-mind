using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;

    private void Awake()
    {
        GetComponent<CanvasGroup>().ToggleCanvasGroup(true);

        startButton.onClick.AddListener(() => InvokeUIType(TypeOfUIWindow.LobbyMenu));
        creditsButton.onClick.AddListener(() => InvokeUIType(TypeOfUIWindow.CreditsMenu));

        FindFirstObjectByType<LobbyManager>().OnLobbyLeaveAddListener(() => InvokeUIType(TypeOfUIWindow.MainMenu));
    }

    private void InvokeUIType(TypeOfUIWindow typeOfUiWindow)
    {
        FindFirstObjectByType<UIManager>().OnUITypeChangeInvoke(typeOfUiWindow);
    }
}
