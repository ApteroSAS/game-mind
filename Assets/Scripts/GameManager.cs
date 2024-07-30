using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public enum GameState
{
    Menu,
    Story,
    Question1,
    Q1Labyrinth,
    Question2,
    Question3,
    Question4,
    Result,
}

public class GameManager : NetworkBehaviour
{
    private Camera mainCamera;

    //Question1
    [SerializeField] private GameObject question1Prefab;
    [SerializeField] private GameObject easyLabyrinth;
    [SerializeField] private GameObject hardLabyrinth;
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject labyrinthCamera;
    //Question2
    [SerializeField] private GameObject question2CakeLayer;

    //Question3
    [SerializeField] private GameObject question3PodestPrefab;
    [SerializeField] private GameObject question3QuestionBlockPrefab;
    [SerializeField] private GameObject question3BlockPrefab;

    //Question4       
    [SerializeField] private GameObject question4Prefab;
    [SerializeField] private GameObject question4WandPrefab;

    public List<GameObject> spawnedInstances = new();

    private GameState currentGameState = GameState.Menu;
    private bool hostReady = false;
    private bool guestReady = false;

    #region GameState

    public delegate void OnGameStateChange(GameState gameState);
    private OnGameStateChange onGameStateChange;

    public void OnGameStateChangeAddListener(OnGameStateChange listener)
    {
        onGameStateChange += listener;
    }
    public void OnGameStateChangeInvoke(GameState gameState)
    {
        onGameStateChange.Invoke(gameState);
    }

    #endregion

    #region PlayerReady

    public delegate void OnPlayerReady(ServerRpcParams serverRpcParams = default);
    private OnPlayerReady onPlayerReady;

    public void OnPlayerReadyAddListener(OnPlayerReady listener)
    {
        onPlayerReady += listener;
    }
    public void OnPlayerReadyInvoke(ServerRpcParams serverRpcParams = default)
    {
        onPlayerReady.Invoke(serverRpcParams);
    }

    #endregion

    private float readyTimer = 0;
    private bool bothReady = false;

    #region OnPlayerReadyReceive

    public delegate void OnPlayerReadyReceive(ResponsibleFor responsibleFor, bool isReady);
    private OnPlayerReadyReceive onPlayerReadyReceive;

    public void OnPlayerReadyReceiveAddListener(OnPlayerReadyReceive listener)
    {
        onPlayerReadyReceive += listener;
    }

    public void OnPlayerReadyReceiveInvoke(ResponsibleFor responsibleFor, bool isReady)
    {
        onPlayerReadyReceive.Invoke(responsibleFor, isReady);
    }

    #endregion

    void Start()
    {
        mainCamera = Camera.main;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        OnPlayerReadyAddListener(TogglePlayerReadyServerRpc);
    }

    private void FixedUpdate()
    {
        if (bothReady) readyTimer += Time.fixedDeltaTime;
        if (readyTimer >= 1)
        {
            readyTimer = 0;
            hostReady = false;
            guestReady = false;
            bothReady = false;
            TogglePlayerReadyClientRpc(true, hostReady);
            TogglePlayerReadyClientRpc(false, guestReady);
            currentGameState++;
            Debug.Log(currentGameState);
            SetGameStateServerRpc(currentGameState);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void TogglePlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        readyTimer = 0;
        var senderClientId = serverRpcParams.Receive.SenderClientId;
        bool senderClientIsHost = senderClientId == NetworkManager.Singleton.LocalClientId;

        if (senderClientIsHost)
        {
            hostReady = !hostReady;
            TogglePlayerReadyClientRpc(senderClientIsHost, hostReady);
        }
        else
        {
            guestReady = !guestReady;
            TogglePlayerReadyClientRpc(senderClientIsHost, guestReady);
        }

        if (hostReady == true && guestReady == true)
        {
            bothReady = true;
        }
    }

    [ClientRpc]
    private void TogglePlayerReadyClientRpc(bool senderClientIsHost, bool isReady)
    {
        if (senderClientIsHost)
        {
            OnPlayerReadyReceiveInvoke(ResponsibleFor.Host, isReady);
        }
        else
        {
            OnPlayerReadyReceiveInvoke(ResponsibleFor.Guest, isReady);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.enabled = false;
            mainCamera.gameObject.SetActive(true);
        }
    }

    private void SpawnQuestion1()
    {
        var question1Instance = Instantiate(question1Prefab);
        spawnedInstances.Add(question1Instance);
    }

    private void SpawnQ1Labyrinth()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                PlayerAnswers hostAnswers = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerAnswers>();
                PlayerAnswers guestAnswers = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>();

                if(hostAnswers.NetworkQ1Answer.Value == guestAnswers.NetworkQ1Answer.Value)
                {
                    var labyrinthInstance = Instantiate(easyLabyrinth);
                    spawnedInstances.Add(labyrinthInstance);

                    NetworkObject guestPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                    Transform teleportSpot = labyrinthInstance.GetComponent<Q1_Labyrinth>().GetTeleportSpot();
                    guestPlayerObject.GetComponent<TeleportOnSpawn>().RotateAndMoveClientRpc(teleportSpot.position, teleportSpot.eulerAngles);

                    var indicatorInstance = Instantiate(indicator, guestPlayerObject.transform);
                    spawnedInstances.Add(indicatorInstance);

                    NetworkObject hostPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
                    var labyrinthCameraInstance = Instantiate(labyrinthCamera);
                    labyrinthCameraInstance.GetComponent<Q1_CameraSize>().ChangeCameraSize();
                    spawnedInstances.Add(labyrinthCameraInstance);
                }
                else
                {
                    var labyrinthInstance = Instantiate(hardLabyrinth);
                    spawnedInstances.Add(labyrinthInstance);

                    NetworkObject guestPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                    Transform teleportSpot = labyrinthInstance.GetComponent<Q1_Labyrinth>().GetTeleportSpot(); 
                    guestPlayerObject.GetComponent<TeleportOnSpawn>().RotateAndMoveClientRpc(teleportSpot.position, teleportSpot.eulerAngles);

                    var indicatorInstance = Instantiate(indicator, guestPlayerObject.transform);
                    spawnedInstances.Add(indicatorInstance);

                    NetworkObject hostPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
                    var labyrinthCameraInstance = Instantiate(labyrinthCamera);
                    spawnedInstances.Add(labyrinthCameraInstance);
                }

                TogglePlayerReadyServerRpc();
            }
        }
    }

    private void SpawnQuestion2()
    {
        var question2Instance = Instantiate(question2CakeLayer);
        spawnedInstances.Add(question2Instance);
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

    [ServerRpc]
    private void SpawnResultsServerRpc()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId) 
            {
                PlayerAnswers hostAnswers = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerAnswers>();
                PlayerAnswers guestAnswers = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>();

                ResultManager resultManager = FindFirstObjectByType<ResultManager>();

                resultManager.CalculateQuestion1(hostAnswers, guestAnswers);

                resultManager.CalculateQuestion3(hostAnswers, guestAnswers);
                resultManager.CalculateQuestion4(hostAnswers, guestAnswers);

                hostAnswers.ShowResults(true);
                guestAnswers.ShowResults(false);
            }
        }
    }



    [ServerRpc(RequireOwnership = true)]
    public void SetGameStateServerRpc(GameState newGameState)
    {
        currentGameState = newGameState;
        Debug.Log("CurrentGameState: " + currentGameState);
        ClearInstancesClientRpc();

        switch (currentGameState)
        {
            case GameState.Question1:
                SpawnQuestion1();
                break;

            case GameState.Q1Labyrinth:
                SpawnQ1Labyrinth();
                break;

            case GameState.Question2:
                SpawnQuestion2();
                break;

            case GameState.Question3:
                SpawnQuestion3();
                break;

            case GameState.Question4:
                SpawnQuestion4();
                break;

            case GameState.Result:
                SpawnResultsServerRpc();
                break;

            default:
                break;
        }

        InvokeGameStateChangeClientRpc(currentGameState);
        foreach (var instance in spawnedInstances)
        {
            if (instance != null)
            {
                if (instance.TryGetComponent<NetworkObject>(out var networkObject)) networkObject.Spawn();
            }
        }
    }

    [ClientRpc]
    private void InvokeGameStateChangeClientRpc(GameState gameState)
    {
        OnGameStateChangeInvoke(gameState);
    }
}
