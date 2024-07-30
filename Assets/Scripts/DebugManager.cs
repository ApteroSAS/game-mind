#if UNITY_EDITOR
using Unity.Netcode;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public void StartSinglePlayer()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Camera.main.gameObject.SetActive(false);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>().EnableCameraAndMovement(true);
        }
    }

    [SerializeField] GameState newGameState;
    
    public void JumpToGameState()
    {
        FindFirstObjectByType<GameManager>().SetGameStateServerRpc(newGameState);
    }

}
#endif
