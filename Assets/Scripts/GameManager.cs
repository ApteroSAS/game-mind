using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField] Vector3[] playerSpawns;
    [SerializeField] GameObject heatingStonePrefab;
    [SerializeField] Vector3[] heatingStoneSpawns;

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
            MoveObjectOnSpawnServerRpc(playerObject.transform, clientId, playerSpawns);

            if (playerObject != null)
            {
                playerObject.GetComponent<PlayerAnswers>().AssignAttributeServerRpc();
            }

            // Spawn and move sex meter
            var heatingStoneInstance = Instantiate(heatingStonePrefab);
            heatingStoneInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            MoveObjectOnSpawnServerRpc(heatingStoneInstance.transform, clientId, heatingStoneSpawns);

            //heatingStonePrefabs[clientId].SetActive(true);
            //heatingStonePrefabs[clientId].GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }

        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void MoveObjectOnSpawnServerRpc(Transform transform, ulong clientId, Vector3[] spawnPoints)
    {
        if ((int)clientId < spawnPoints.Length)
        {
            Vector3 spawnPoint = spawnPoints[(int)clientId];
            MoveObjectOnServerRpc(transform, spawnPoint);
            Debug.Log($"Spawned object for client {clientId} at position {spawnPoint}");
        }
        else
        {
            Debug.LogError($"Spawn point not defined for client ID {clientId}");
        }
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
