using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Q1_Help : MonoBehaviour
{
    [SerializeField] Button button;

    CanvasGroup canvasGroup;
    bool isVisible = true;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        button.onClick.AddListener(ToggleCanvas);
    }

    private void ToggleCanvas()
    {
        isVisible = !isVisible;

        canvasGroup.ToggleCanvasGroup(isVisible);
    }


}
