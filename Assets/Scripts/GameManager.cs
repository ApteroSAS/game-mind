using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        // Subscribe to the event when a player is spawned
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    //void OnDestroy()
    //{
    //    // Unsubscribe to prevent memory leaks
    //    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //}

    private void OnClientConnected(ulong clientId)
    {
        // Check if the local client is the one that connected
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Deactivate the main camera once the local player is connected
            mainCamera.gameObject.SetActive(false);
        }
    }
}

