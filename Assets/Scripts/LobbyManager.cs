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
    int maxPlayers = 2;

    public delegate void OnUITypeChange(TypeOfUIWindow typeOfUIWindow);
    private OnUITypeChange onUITypeChange;

    public void OnUITypeChangeAddListener(OnUITypeChange listener)
    {
        onUITypeChange += listener;
    }

    public void OnUITypeChangeInvoke(TypeOfUIWindow typeOfUIWindow)
    {
        onUITypeChange.Invoke(typeOfUIWindow);
    }

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
        FindFirstObjectByType<LobbyPanel>().OnLobbyPanelChangeInvoke(LobbyPanelUI.AfterCreation);
    }

    private async Task JoinRelayServer()
    {
        string joinCode = currentLobby.Data["RelayCode"].Value;
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(joinAllocation, buildTypeData));

        NetworkManager.Singleton.StartClient();
        OnLobbyCreationInvoke(currentLobby.LobbyCode);
        FindFirstObjectByType<LobbyPanel>().OnLobbyPanelChangeInvoke(LobbyPanelUI.AfterCreation);

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

                FindFirstObjectByType<LobbyPanel>().OnLobbyPanelChangeInvoke(LobbyPanelUI.Start);
                OnUITypeChangeInvoke(TypeOfUIWindow.MainMenu);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
