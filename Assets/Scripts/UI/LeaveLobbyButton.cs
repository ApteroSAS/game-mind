using UnityEngine;
using UnityEngine.UI;

public class LeaveLobbyButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(FindFirstObjectByType<LobbyManager>().LeaveLobby);
    }

}
