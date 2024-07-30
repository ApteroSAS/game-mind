using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class Q_Info : NetworkBehaviour
{
    [SerializeField] string feedBackGood;

    [SerializeField] TextMeshProUGUI feedBackMesh;
    
    private Button[] buttons;
    private CanvasGroup canvasGroup;

    private bool isVisible = false;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();

        foreach (var item in buttons)
        {
            item.onClick.AddListener(ToggleVisible);
        }
    }

    private void ToggleVisible()
    {
        isVisible = !isVisible;
        canvasGroup.ToggleCanvasGroup(isVisible);
    }

    [ClientRpc]
    public void SetFeedBackClientRpc(bool isFeedBackGood)
    {
        if (isFeedBackGood)
        {
            feedBackMesh.text = feedBackGood;
        }
    }
}
