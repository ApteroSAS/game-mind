using TMPro;
using UnityEngine;

public class JoinLobbyInputField : MonoBehaviour
{
    private bool isActive = false;

    private void Awake()
    {
        FindFirstObjectByType<LobbyManager>().OnUITypeChangeAddListener(IsActiveBasedOnUI);
    }

    private void Update()
    {
        if (isActive) 
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                string lobbyCode = GetComponent<TMP_InputField>().text;
                FindFirstObjectByType<LobbyManager>().JoinLobby(lobbyCode);
            }
        }
    }

    private void IsActiveBasedOnUI(TypeOfUIWindow typeOfUIWindow)
    {
        if (typeOfUIWindow == TypeOfUIWindow.LobbyMenu)
        {
            isActive = true;
        }
        else isActive = false;
    }
}
