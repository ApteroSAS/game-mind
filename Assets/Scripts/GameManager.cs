using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using System.Collections.Generic;

public enum GameState
{
    Tutorial,
    Question1,
    Question2,
    Question3,
    Question4,
    End,
}

public class GameManager : NetworkBehaviour
{
    public Camera mainCamera;

    [SerializeField] public Transform playerSpawn;
    [SerializeField] GameObject question4Prefab;
    [SerializeField] GameObject wandPrefab;
    [SerializeField] Transform question4Spawn;

    private List<GameObject> listOfSpawnedObjects = new List<GameObject>();


    public NetworkVariable<GameState> NetworkGameState = new NetworkVariable<GameState>(GameState.Tutorial);

    private void Update()
    {
        if (IsServer)
        {
            SetGameStateServerRpc();
        }
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            NetworkGameState.Value = GameState.Question1;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            //SpawnCauldron(clientId);
        }

        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }

    private void SpawnQuestion4()
    {
            var question4Instance = Instantiate(question4Prefab);
            listOfSpawnedObjects.Add(question4Instance);
            question4Instance.GetComponent<NetworkObject>().Spawn();

            //foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            //{
            //    PlayerAttribute attribute = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
            //    var wandInstance = Instantiate()
            //}
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetGameStateServerRpc()
    {
        GameState previousGameState = NetworkGameState.Value;
        if (Input.GetKeyDown(KeyCode.Alpha1)) NetworkGameState.Value = GameState.Question1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) NetworkGameState.Value = GameState.Question2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) NetworkGameState.Value = GameState.Question3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) NetworkGameState.Value = GameState.Question4;

        if (previousGameState == NetworkGameState.Value)
            return;

        for (int i = 0; i < listOfSpawnedObjects.Count; i++)
        {
            listOfSpawnedObjects[i].GetComponent<NetworkObject>().Despawn();
        }

        listOfSpawnedObjects.Clear();

        switch (NetworkGameState.Value)
        {
            case GameState.Tutorial:
                break;

            case GameState.Question1:
                break;

            case GameState.Question2:
                break;

            case GameState.Question3:
                break;

            case GameState.Question4:
                SpawnQuestion4();
                break;

            case GameState.End:
                break;

            default:
                break;
        }
    }

}
