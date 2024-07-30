using Unity.VisualScripting;
using UnityEngine;

public static class UIExtentions
{
    public static void ToggleCanvasGroup(this CanvasGroup canvasGroup, bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.interactable = toggle;
        canvasGroup.blocksRaycasts = toggle;
    }
}
