using UnityEngine;
using UnityEngine.UI;
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
using TMPro;

public enum BuildType
{
    Windows,
    WebGL,
}

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TMP_InputField joinLobbyInputField;

    private Lobby currentLobby;

    public delegate void OnUITypeChange(TypeOfUIWindow typeOfUIWindow);
    public OnUITypeChange onUITypeChange;

    public delegate void OnLobbyCreation(string code);
    public OnLobbyCreation onLobbyCreation;

    [SerializeField] private BuildType buildType;
    private string buildTypeData;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);

        createLobbyButton.onClick.AddListener(CreateLobby);
        joinLobbyButton.onClick.AddListener(JoinLobby);

        switch (buildType)
        {
            case BuildType.Windows:
                buildTypeData = "dtsl";
                break;
            case BuildType.WebGL:
                buildTypeData = "wss";
                break;
            default:
                break;
        }
    }

    private async void CreateLobby()
    {
        string lobbyName = "";
        int maxPlayers = 2; // example max players

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

    private async void JoinLobby()
    {
        string lobbyCode = joinLobbyInputField.text;

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
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
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
        onLobbyCreation.Invoke(currentLobby.LobbyCode);
        onUITypeChange.Invoke(TypeOfUIWindow.StoryMenu);
        FindFirstObjectByType<GameManager>().SetGameStateServerRpc(GameState.Story);
    }

    private async Task JoinRelayServer()
    {
        string joinCode = currentLobby.Data["RelayCode"].Value;
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(joinAllocation, buildTypeData));

        NetworkManager.Singleton.StartClient();
        onUITypeChange.Invoke(TypeOfUIWindow.StoryMenu);
        FindFirstObjectByType<GameManager>().onGameStateChange.Invoke(GameState.Story); //makes it local?
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

                onUITypeChange.Invoke(TypeOfUIWindow.LobbyMenu);
                FindFirstObjectByType<GameManager>().SetGameStateServerRpc(GameState.LobbyMenu);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
