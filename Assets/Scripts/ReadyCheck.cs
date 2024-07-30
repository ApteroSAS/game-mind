using UnityEngine;
using UnityEngine.UI;

public enum ResponsibleFor
{
    Host,
    Guest,
}

public class ReadyCheck : MonoBehaviour
{
    [SerializeField] private ResponsibleFor player;
    [SerializeField] private Image image;

    private Color redColor = new Color(255/255f, 124/255f, 128/255f, 255/255f);
    private Color greenColor = new Color(140/255f, 206/255f, 101/255f, 255/255f);

    private void Awake()
    {
        image.color = redColor;

        FindFirstObjectByType<GameManager>().OnPlayerReadyReceiveAddListener(ToggleColor);
    }

    private void ToggleColor(ResponsibleFor responsibleFor, bool isReady)
    {
        if (responsibleFor != player) return;
        if (isReady)
        {
            image.color = greenColor;
        }
        else
            image.color = redColor;
    }
}
