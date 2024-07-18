using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public enum GameState
{
    LobbyMenu,
    Story,
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

    private bool player1Ready = false;
    private bool player2Ready = false;

    //Question3
    [SerializeField] private GameObject question3PodestPrefab;
    [SerializeField] private GameObject question3QuestionBlockPrefab;
    [SerializeField] private GameObject question3BlockPrefab;

    //Question4       
    [SerializeField] private GameObject question4Prefab;
    [SerializeField] private GameObject question4WandPrefab;

    private List<GameObject> spawnedInstances = new();

    public NetworkVariable<GameState> NetworkGameState = new NetworkVariable<GameState>(GameState.Tutorial);

    public delegate void OnGameStateChange(GameState gameState);
    public OnGameStateChange onGameStateChange;

    public delegate void OnClientJoinLobby();
    public OnClientJoinLobby onClientJoinLobby;

    public delegate void OnPlayerReadyCheck(ServerRpcParams serverRpcParams = default);
    public OnPlayerReadyCheck onPlayerReadyCheck;

    public delegate void OnPlayerReadySend(ResponsibleFor responsibleFor, bool isReady);
    public OnPlayerReadySend onPlayerReadySend;



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

        onPlayerReadyCheck += TogglePlayerReadyServerRpc;
    }

    [ServerRpc (RequireOwnership = false)]
    private void TogglePlayerReadyServerRpc(ServerRpcParams serverRpcParams)
    {
        var senderClientId = serverRpcParams.Receive.SenderClientId;
        bool senderClientIsHost = senderClientId == NetworkManager.Singleton.LocalClientId;
        TogglePlayerReadyClientRpc(senderClientId, senderClientIsHost);
    }

    [ClientRpc]
    private void TogglePlayerReadyClientRpc(ulong senderClientId, bool senderClientIsHost)
    {
        if (senderClientIsHost)
        {
            player1Ready = !player1Ready;
            Debug.Log("Player " + senderClientId + " is ready? " + player1Ready);
            onPlayerReadySend.Invoke(ResponsibleFor.Host, player1Ready);
        }
        else
        {
            player2Ready = !player2Ready;
            Debug.Log("Player " + senderClientId + " is ready? " + player2Ready);
            onPlayerReadySend.Invoke(ResponsibleFor.Guest, player2Ready);
        }

        if(player1Ready == true && player2Ready == true)
        {

        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mainCamera.gameObject.SetActive(false);
        }
        else onClientJoinLobby.Invoke();
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
        var question3PodestInstance = Instantiate(question3PodestPrefab);
        spawnedInstances.Add(question3PodestInstance);
        var question3QuestionBlockInstance = Instantiate(question3QuestionBlockPrefab);
        spawnedInstances.Add(question3QuestionBlockInstance);
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
            var question3box = Instantiate(question3BlockPrefab);
            spawnedInstances.Add(question3box);
            Vector3 question3Pos = question3box.transform.position;
            question3Pos.y += 1 + (0.5f * i);
            question3box.transform.position = question3Pos;
            question3box.GetComponent<Q3_Block>().OnInstantiate(i, randomBools[i]);
        }
    }

    private void SpawnQuestion4()
    {
        var question4Instance = Instantiate(question4Prefab);
        spawnedInstances.Add(question4Instance);
        InstantiateWandsClientRpc();
    }

    [ClientRpc]
    private void InstantiateWandsClientRpc()
    {
        GameObject playerObject = NetworkManager.LocalClient.PlayerObject.gameObject;
        PlayerAttribute attribute = playerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
        var question4wand = Instantiate(question4WandPrefab, playerObject.transform);
        spawnedInstances.Add(question4wand);
        question4wand.GetComponent<Q4_Wand>().SetWandColor(attribute);
    }

    [ClientRpc]
    private void ClearInstancesClientRpc()
    {
        foreach (var instance in spawnedInstances)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }
        spawnedInstances.Clear();
    }

    [ClientRpc]
    private void SaveAnswersClientRpc(GameState previousGameState)
    {
        if(previousGameState == GameState.Question3)
        {
            PlayerAnswers answers = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerAnswers>();
            answers.Q3Blocks.Clear();
            foreach (var item in spawnedInstances)
            {
                Q3_Block q3_block = item.GetComponent<Q3_Block>();
                if (q3_block == null) continue;

                Q3_HoldData q3_data = new Q3_HoldData(q3_block.GetSymbol(), item.transform.position);
                answers.Q3Blocks.Add(q3_data);
            }
            answers.Q3Blocks.Sort((a,b) => a.PositionData.y.CompareTo(b.PositionData.y));
        }
    }

    private void StartNextGameState()
    {

    }

    [ServerRpc(RequireOwnership = true)]
    public void SetGameStateServerRpc(GameState newGameState)
    {
        GameState previousGameState = NetworkGameState.Value;
        NetworkGameState.Value = newGameState;
        if (previousGameState == newGameState)
            return;

        SaveAnswersClientRpc(previousGameState);
        ClearInstancesClientRpc();

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

        InvokeGameStateChangeClientRpc(newGameState);
        foreach (var instance in spawnedInstances)
        {
            if (instance != null)
            {
                NetworkObject networkObject = instance.GetComponent<NetworkObject>();
                if (networkObject != null) networkObject.Spawn();
            }
        }
    }

    [ClientRpc]
    private void InvokeGameStateChangeClientRpc(GameState gameState)
    {
        onGameStateChange.Invoke(gameState);
    }

}
