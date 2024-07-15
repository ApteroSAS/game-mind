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
    [SerializeField] private GameObject blockPrefab;

    //Question4       
    [SerializeField] private GameObject question4Prefab;
    [SerializeField] private GameObject wandPrefab;


    public List<GameObject> listOfSpawnedObjects = new List<GameObject>();
    public delegate void DestroyLocalObject();
    public DestroyLocalObject destroyLocalObject;

    public NetworkVariable<GameState> NetworkGameState = new NetworkVariable<GameState>(GameState.Tutorial);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) if (IsServer) SetGameStateServerRpc(GameState.Question1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) if (IsServer) SetGameStateServerRpc(GameState.Question2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) if (IsServer) SetGameStateServerRpc(GameState.Question3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) if (IsServer) SetGameStateServerRpc(GameState.Question4);
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
        bool[] randomBools = new bool[6];
        int countTrue = 0;
        int countFalse = 0;
        for (int i = 0; i < randomBools.Length; i++)
        {
            randomBools[i] = Utils.GetRandomBool();
            if (countTrue >= 3) randomBools[i] = false;
            if (countFalse >= 3) randomBools[i] = true;

            if (randomBools[i] == true) 
                countTrue++;
            else 
                countFalse++;
        }
        InstantiateBoxesClientRpc(randomBools);
    }

    [ClientRpc]
    private void InstantiateBoxesClientRpc(bool[] randomBools)
    {
        for (int i = 0; i < 6; i++)
        {
            var question3box = Instantiate(blockPrefab);
            Vector3 question3Pos = question3box.transform.position;
            question3Pos.y += 1 + (0.5f * i);
            question3box.transform.position = question3Pos;
            question3box.GetComponent<Q3_Block>().OnInstantiate(i, randomBools[i]);
            destroyLocalObject += question3box.GetComponent<Q3_Block>().SetDestroy;
        }
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
    private void SetGameStateServerRpc(GameState newGameState)
    {
        GameState previousGameState = NetworkGameState.Value;
        NetworkGameState.Value = newGameState;
        if (previousGameState == newGameState)
            return;

        for (int i = 0; i < listOfSpawnedObjects.Count; i++)
        {
            if(listOfSpawnedObjects[i] != null)
            listOfSpawnedObjects[i].GetComponent<NetworkObject>().Despawn();
        }

        listOfSpawnedObjects.Clear();
        if (destroyLocalObject != null)
        {
            DestroyPropsClientRpc();
            destroyLocalObject = null;
        }

        switch (newGameState)
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
