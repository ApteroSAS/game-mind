using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField] Button returnButton;

    private void Awake()
    {
        returnButton.onClick.AddListener(ReturnButton);
    }

    private void ReturnButton()
    {
        FindFirstObjectByType<UIManager>().OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
    }
}
