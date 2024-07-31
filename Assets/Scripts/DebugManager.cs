#if UNITY_EDITOR
using Unity.Netcode;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

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

            gameManager.SetSinglePlayerMode(true);
            gameManager.SetGameStateServerRpc(GameState.Story);
        }
    }

    [SerializeField] GameState newGameState;
    
    public void JumpToGameState()
    {
        gameManager.SetGameStateServerRpc(newGameState);
    }

}
#endif
