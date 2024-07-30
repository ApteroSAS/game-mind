using UnityEngine;

public class ToggleConsole : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isVisible = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Caret) || Input.GetKeyDown(KeyCode.P))
        {
            isVisible = !isVisible;
            GetComponent<CanvasGroup>().ToggleCanvasGroup(isVisible);
        }
    }
}
