using UnityEngine;


public enum TypeOfUIWindow
{
    LobbyMenu,
    InGameMenu,
    StoryMenu,
    TutorialMenu,
}

[RequireComponent(typeof(CanvasGroup))]
public class UIType : MonoBehaviour
{
    [SerializeField] private TypeOfUIWindow typeOfUIWindow;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        FindFirstObjectByType<LobbyManager>().onUITypeChange += ToggleCanvasGroup;
    }

    private void ToggleCanvasGroup(TypeOfUIWindow newtypeOfUIWindow)
    {
        //switch (newtypeOfUIWindow)
        //{
        //    case TypeOfUIWindow.LobbyMenu:
        //        canvasGroup.ToggleCanvasGroup(newtypeOfUIWindow == typeOfUIWindow);
        //        break;
        //    case TypeOfUIWindow.InGameMenu:
        //        canvasGroup.ToggleCanvasGroup(newtypeOfUIWindow == typeOfUIWindow);
        //        break;
        //    case TypeOfUIWindow.StoryMenu:
        //        canvasGroup.ToggleCanvasGroup(newtypeOfUIWindow == typeOfUIWindow);
        //        break;
        //    case TypeOfUIWindow.TutorialMenu:
        //        canvasGroup.alpha = 1;
        //        break;
        //    default:
        //        break;
        //}
        canvasGroup.ToggleCanvasGroup(newtypeOfUIWindow == typeOfUIWindow);
    }


}
