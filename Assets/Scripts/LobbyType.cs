using UnityEngine;


public enum TypeOfLobbyWindow
{
    LOBBYMENU,
    INLOBBYMENU,
}

public class LobbyType : MonoBehaviour
{


    [SerializeField] private TypeOfLobbyWindow typeOfLobbyWindow;

    public TypeOfLobbyWindow GetLobbyType()
    {
        return typeOfLobbyWindow;
    }


}
