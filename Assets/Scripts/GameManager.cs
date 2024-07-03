using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField] Transform playerSpawn;
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
            MoveObjectOnSpawnServerRpc(playerObject.transform, clientId, playerSpawn.position);

            //if (playerObject != null)
            //    playerObject.GetComponent<PlayerAnswers>().AssignAttributeServerRpc();

            SpawnHeatingStone(clientId);
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

    private void SpawnHeatingStone(ulong clientId)
    {
        // Spawn and move heatingStone
        var heatingStoneInstance = Instantiate(heatingStonePrefab, heatingStoneSpawn, Quaternion.identity);
        heatingStoneInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        //MoveObjectOnSpawnServerRpc(heatingStoneInstance.transform, clientId, heatingStoneSpawn);
    }

}
