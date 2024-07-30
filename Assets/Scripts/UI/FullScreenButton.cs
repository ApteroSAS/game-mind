using UnityEngine;
using UnityEngine.UI;

public class FullScreenButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private Sprite fullScreen;
    [SerializeField] private Sprite smallScreen;

    private bool isFullScreen;

    private void Awake()
    {
        isFullScreen = Screen.fullScreen;

        button.onClick.AddListener(ToggleFullScreen);
    }

    private void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;

        if (isFullScreen) image.sprite = smallScreen;
        else image.sprite = fullScreen;
        
        Screen.fullScreen = isFullScreen;
    }

        
}
