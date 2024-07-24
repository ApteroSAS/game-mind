using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public enum GameState
{
    LobbyMenu,
    Story,
    Question1,
    Question2,
    Question3,
    Question4,
    Result,
}

public class GameManager : NetworkBehaviour
{
    public Camera mainCamera;

    [SerializeField] public Transform playerSpawn;

    //Question3
    [SerializeField] private GameObject question3PodestPrefab;
    [SerializeField] private GameObject question3QuestionBlockPrefab;
    [SerializeField] private GameObject question3BlockPrefab;

    //Question4       
    [SerializeField] private GameObject question4Prefab;
    [SerializeField] private GameObject question4WandPrefab;

    private List<GameObject> spawnedInstances = new();

    public NetworkVariable<GameState> NetworkGameState = new NetworkVariable<GameState>();
    private NetworkVariable<bool> NetworkPlayer1Ready = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> NetworkPlayer2Ready = new NetworkVariable<bool>(false);

    public delegate void OnGameStateChange(GameState gameState);
    public OnGameStateChange onGameStateChange;

    public delegate void OnClientJoinLobby();
    public OnClientJoinLobby onClientJoinLobby;

    public delegate void OnPlayerReadyCheck(ServerRpcParams serverRpcParams = default);
    public OnPlayerReadyCheck onPlayerReadyCheck;

    public delegate void OnPlayerReadySend(ResponsibleFor responsibleFor, bool isReady);
    public OnPlayerReadySend onPlayerReadySend;

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

        Debug.Log("TogglePlayerReadyServerRpc got triggerd by player " + senderClientId);

        if (senderClientIsHost)
        {
            NetworkPlayer1Ready.Value = !NetworkPlayer1Ready.Value;
            TogglePlayerReadyClientRpc(senderClientIsHost, NetworkPlayer1Ready.Value);
        }
        else
        {
            NetworkPlayer2Ready.Value = !NetworkPlayer2Ready.Value;
            TogglePlayerReadyClientRpc(senderClientIsHost, NetworkPlayer2Ready.Value);
        }


        if (NetworkPlayer1Ready.Value == true && NetworkPlayer2Ready.Value == true)
        {
            NetworkPlayer1Ready.Value = false;
            NetworkPlayer2Ready.Value = false;
            TogglePlayerReadyClientRpc(true, false);
            TogglePlayerReadyClientRpc(false, false);
            GameState currentGameState = NetworkGameState.Value;
            currentGameState++;
            Debug.Log("current: " + NetworkGameState.Value + " new: " + currentGameState);
            SetGameStateServerRpc(currentGameState);
        }
    }

    [ClientRpc]
    private void TogglePlayerReadyClientRpc(bool senderClientIsHost, bool isReady)
    {
        if (senderClientIsHost)
        {
            onPlayerReadySend.Invoke(ResponsibleFor.Host, isReady);
        }
        else
        {
            onPlayerReadySend.Invoke(ResponsibleFor.Guest, isReady);
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
            List<Q3_HoldData> q3_blocks = new();

            foreach (var item in spawnedInstances)
            {
                Q3_Block q3_block = item.GetComponent<Q3_Block>();
                if (q3_block == null) continue;

                Q3_HoldData q3_data = new Q3_HoldData(q3_block.GetSymbol(), item.transform.position);
                q3_blocks.Add(q3_data);

            }
            q3_blocks.Sort((a,b) => a.PositionData.y.CompareTo(b.PositionData.y));
            foreach (var item in q3_blocks)
            {
                answers.Q3Blocks.Add(item);
            }
        }
    }

    [ClientRpc]
    private void SpawnResultsClientRpc()
    {
        PlayerAnswers hostAnswers = NetworkManager.Singleton.ConnectedClients[0].PlayerObject.GetComponent<PlayerAnswers>();
        PlayerAnswers guestAnswers = NetworkManager.Singleton.ConnectedClients[1].PlayerObject.GetComponent<PlayerAnswers>();

        hostAnswers.ShowResults(true);
        guestAnswers.ShowResults(false);
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
            case GameState.Question1:
                SetGameStateServerRpc(GameState.Question2);
                break;

            case GameState.Question2:
                SetGameStateServerRpc(GameState.Question3);
                break;

            case GameState.Question3:
                SpawnQuestion3();
                break;

            case GameState.Question4:
                SpawnQuestion4();
                break;

            case GameState.Result:
                SpawnResultsClientRpc();
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
