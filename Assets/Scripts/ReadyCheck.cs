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

    private void Awake()
    {
        image.color = Color.red;

        FindFirstObjectByType<GameManager>().onPlayerReadySend += ToggleColor;
    }

    private void ToggleColor(ResponsibleFor responsibleFor, bool isReady)
    {
        if (responsibleFor != player) return;
        if (isReady)
        {
            image.color = Color.green;
        }
        else
            image.color = Color.red;
    }
}
