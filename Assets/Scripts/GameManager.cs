using UnityEngine;
using Unity.Netcode;
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

    //Question3
    [SerializeField] private GameObject question3Prefab;
                      
    //Question4       
    [SerializeField] private GameObject question4Prefab;
    [SerializeField] private GameObject wandPrefab;


    public List<GameObject> listOfSpawnedObjects = new List<GameObject>();
    public delegate void DestroyLocalObject();
    public DestroyLocalObject destroyLocalObject;

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
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            NetworkGameState.Value = GameState.Question1;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mainCamera.gameObject.SetActive(true);
        }
    }

    private void SpawnQuestion3()
    {
        var question3Instance = Instantiate(question3Prefab);
        listOfSpawnedObjects.Add(question3Instance);
        question3Instance.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    private void InstantiateCubesClientRpc()
    {
        GameObject playerObject = NetworkManager.LocalClient.PlayerObject.gameObject;
        PlayerAttribute attribute = playerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
        var question4wand = Instantiate(wandPrefab, playerObject.transform);
    }

    private void SpawnQuestion4()
    {
        var question4Instance = Instantiate(question4Prefab);
        listOfSpawnedObjects.Add(question4Instance);
        question4Instance.GetComponent<NetworkObject>().Spawn();
        InstantiateWandsClientRpc();
    }

    [ClientRpc]
    private void InstantiateWandsClientRpc()
    {
        GameObject playerObject = NetworkManager.LocalClient.PlayerObject.gameObject;
        PlayerAttribute attribute = playerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
        var question4wand = Instantiate(wandPrefab, playerObject.transform);
        question4wand.GetComponent<Q4_Wand>().SetWandColor(attribute);
        destroyLocalObject += question4wand.GetComponent<Q4_Wand>().SetDestroy;
    }

    [ClientRpc]
    private void DestroyPropsClientRpc()
    {
        destroyLocalObject.Invoke();
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
        if (destroyLocalObject != null)
        {
            DestroyPropsClientRpc();
            destroyLocalObject = null;
        }

        switch (NetworkGameState.Value)
        {
            case GameState.Tutorial:
                break;

            case GameState.Question1:
                break;

            case GameState.Question2:
                break;

            case GameState.Question3:
                SpawnQuestion3();
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
