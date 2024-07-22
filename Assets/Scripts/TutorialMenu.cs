using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool darken = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        FindFirstObjectByType<LobbyManager>().onUITypeChange += OpenTutorialMenu;
    }

    private void Update()
    {
        if(darken)
        {
            canvasGroup.alpha -= Time.deltaTime * 0.1f;
            if (canvasGroup.alpha <= 0) 
            {
                darken = false;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    private void OpenTutorialMenu(TypeOfUIWindow UIWindow) 
    {
        if (UIWindow == TypeOfUIWindow.TutorialMenu) darken = true;
    }
}