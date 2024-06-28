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

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TMP_InputField joinLobbyInputField;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private UILobby uiLobby;

    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private Transform[] spawnPoints;

    private Lobby currentLobby;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);

        createLobbyButton.onClick.AddListener(CreateLobby);
        joinLobbyButton.onClick.AddListener(JoinLobby);
    }

    private async void CreateLobby()
    {
        string lobbyName = lobbyNameInputField.text;
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

            //for (int i = 0; i < objectsToSpawn.Length; i++)
            //{
            //    GameObject instantiatedObject = Instantiate(objectsToSpawn[i]);
            //    instantiatedObject.GetComponent<NetworkObject>().Spawn();
            //    //instantiatedObject.transform.position = spawnPoints[i].position;
            //}

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
        transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

        NetworkManager.Singleton.StartHost();
        lobbyCode.text = currentLobby.LobbyCode.ToString();
        uiLobby.SetCurrentLobby(TypeOfLobbyWindow.INLOBBYMENU);
    }

    private async Task JoinRelayServer()
    {
        string joinCode = currentLobby.Data["RelayCode"].Value;
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        NetworkManager.Singleton.StartClient();
        lobbyCode.text = currentLobby.LobbyCode.ToString();
        uiLobby.SetCurrentLobby(TypeOfLobbyWindow.INLOBBYMENU);
    }
}
