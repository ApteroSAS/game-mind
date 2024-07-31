using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;

public enum BuildType
{
    Windows,
    WebGL,
}

public class LobbyManager : MonoBehaviour
{
    private Lobby currentLobby;
    private readonly int maxPlayers = 2;

    private float timer = 0f;
    private readonly float timerCheckServer = 5f;


    public delegate void OnLobbyCreation(string code);
    private OnLobbyCreation onLobbyCreation;

    public void OnLobbyCreationAddListener(OnLobbyCreation listener)
    {
        onLobbyCreation += listener;
    }

    public void OnLobbyCreationInvoke(string code)
    {
        onLobbyCreation.Invoke(code);
    }

    public delegate void OnLobbyJoin(string code);
    private OnLobbyJoin onLobbyJoin;

    public void OnLobbyJoinAddListener(OnLobbyJoin listener)
    {
        onLobbyJoin += listener;
    }

    public void OnLobbyJoinInvoke(string code)
    {
        onLobbyJoin.Invoke(code);
    }

    public delegate void OnLobbyLeave();
    private OnLobbyLeave onLobbyLeave;

    public void OnLobbyLeaveAddListener(OnLobbyLeave listener)
    {
        onLobbyLeave += listener;
    }

    private void OnLobbyLeaveInvoke()
    {
        onLobbyLeave.Invoke();
    }

    [SerializeField] private BuildType buildType;
    private string buildTypeData;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);

        switch (buildType)
        {
            case BuildType.Windows:
                buildTypeData = "dtls";
                break;
            case BuildType.WebGL:
                buildTypeData = "wss";
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        bool insideLobby = currentLobby != null;
        Debug.Log("I'm inside a lobby:" + insideLobby);
        if (!insideLobby) return;

        timer += Time.deltaTime;
        if(timer >= timerCheckServer)
        {
            timer = 0f;
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Someone left server and I'm alone");
                OnLobbyLeaveInvoke();
            }
        }
    }

    public async void CreateLobby()
    {
        string lobbyName = "";

        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, "", DataObject.IndexOptions.S1) }
                }
            };

            if (lobbyName == "") lobbyName = Random.Range(100, 999).ToString();
            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("Lobby created: " + currentLobby.Id);

            // Start the server and set up Relay
            await StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log("Joined lobby: " + currentLobby.Id);

            // Join the Relay server as a client
            await JoinRelayServer();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task StartHost()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // Update lobby data with the new Relay join code
        var updatedData = new Dictionary<string, DataObject>(currentLobby.Data)
        {
            ["RelayCode"] = new DataObject(DataObject.VisibilityOptions.Member, joinCode, DataObject.IndexOptions.S1)
        };

        currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
        {
            Data = updatedData
        });

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(allocation, buildTypeData));

        NetworkManager.Singleton.StartHost();
        OnLobbyCreationInvoke(currentLobby.LobbyCode);
    }

    private async Task JoinRelayServer()
    {
        string joinCode = currentLobby.Data["RelayCode"].Value;
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(joinAllocation, buildTypeData));

        NetworkManager.Singleton.StartClient();
        OnLobbyJoinInvoke(currentLobby.LobbyCode);

    }

    public async void LeaveLobby()
    {
        try
        {
            if (currentLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("Left the lobby: " + currentLobby.Id);
                currentLobby = null;

                // Check if we are the host or client and stop the network accordingly
                if (NetworkManager.Singleton.IsHost)
                {
                    NetworkManager.Singleton.Shutdown();
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.Shutdown();
                }
                OnLobbyLeaveInvoke();
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
