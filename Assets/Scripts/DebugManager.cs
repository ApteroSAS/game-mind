#if UNITY_EDITOR
using Unity.Netcode;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public void StartHost()
    {
        if(!NetworkManager.Singleton.IsServer)
            FindFirstObjectByType<LobbyManager>().CreateLobby();
    }

    public void StartSinglePlayer()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Camera.main.gameObject.SetActive(false);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>().EnableCameraAndMovement(true);

            FindFirstObjectByType<GameManager>().singlePlayerMode = true;
            FindFirstObjectByType<GameManager>().SetGameStateServerRpc(GameState.Story);
        }
    }

    [SerializeField] GameState newGameState;
    
    public void JumpToGameState()
    {
        FindFirstObjectByType<GameManager>().SetGameStateServerRpc(newGameState);
    }

}
#endif
