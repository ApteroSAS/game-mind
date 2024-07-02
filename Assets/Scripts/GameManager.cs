using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField] Vector3 playerSpawn;
    [SerializeField] GameObject heatingStonePrefab;
    [SerializeField] Vector3 heatingStoneSpawn;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // go through all clients and set proper spawn points pls thanks
        }
    }

    void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            // Spawn and move player
            MoveObjectOnSpawnServerRpc(playerObject.transform, clientId, playerSpawn);

            //if (playerObject != null)
            //    playerObject.GetComponent<PlayerAnswers>().AssignAttributeServerRpc();

            // Spawn and move heatingStone
            var heatingStoneInstance = Instantiate(heatingStonePrefab);
            heatingStoneInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            MoveObjectOnSpawnServerRpc(heatingStoneInstance.transform, clientId, heatingStoneSpawn);
        }

        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void MoveObjectOnSpawnServerRpc(Transform transform, ulong clientId, Vector3 spawnPoint)
    {
            MoveObjectOnServerRpc(transform, spawnPoint);
            Debug.Log($"Spawned object for client {clientId} at position {spawnPoint}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveObjectOnServerRpc(Transform transform, Vector3 newPosition)
    {
        transform.position = newPosition;

        MoveObjectOnClientRpc(transform, newPosition);
    }

    [ClientRpc]
    private void MoveObjectOnClientRpc(Transform transform, Vector3 newPosition)
    {
        transform.position = newPosition;
    }

}
