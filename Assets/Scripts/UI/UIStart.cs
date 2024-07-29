using UnityEngine;

public class UIStart : MonoBehaviour
{
    void Start()
    {
        GetComponent<CanvasGroup>().ToggleCanvasGroup(true);
    }

}
