using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartBeatTimer;
    private float lobbyUpdateTimer;
    private float guestJoinedTimer;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text codeTextComponent;
    [SerializeField] LobbyType[] lobbyTypes;

    private async void Start()
    {

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //can put account later, for example steam or itchio
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
        HandleLobbyPollForUpdates();
        CheckIfGuestJoined();
    }

    private void CheckIfGuestJoined()
    {
        if (hostLobby != null)
        {
            guestJoinedTimer -= Time.deltaTime;
            if (guestJoinedTimer < 0f)
            {
                float guestJoinedTimerMax = 1f;
                guestJoinedTimer = guestJoinedTimerMax;
            }
        }

    }

    private async void HandleLobbyHeartBeat()
    {
        if(hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartBeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }


    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 2;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(true)
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            Debug.Log(lobby.Name + " " +  lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            PrintPlayers(hostLobby);

            //activate new window and show code

            codeTextComponent.text = hostLobby.LobbyCode.ToString();
            for (int i = 0; i < lobbyTypes.Length; i++)
            {
                if (lobbyTypes[i].GetLobbyType() == TypeOfLobbyWindow.INLOBBYMENU)
                {
                    OpenProperLobbyWindow(lobbyTypes[i]);
                }
            }

        } catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 5,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false,QueryOrder.FieldOptions.Created),
                },
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        } catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void JoinLobbyByCode()
    {
        try
        {
            string lobbyCode = inputField.text;

            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(false)
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);


            codeTextComponent.text = joinedLobby.LobbyCode.ToString();
            for (int i = 0; i < lobbyTypes.Length; i++)
            {
                if (lobbyTypes[i].GetLobbyType() == TypeOfLobbyWindow.INLOBBYMENU)
                {
                    OpenProperLobbyWindow(lobbyTypes[i]);
                }
            }
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    
    public void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name + ":");
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private Player GetPlayer(bool isHost)
    {
        string playerName = isHost? "Host" :"Guest";
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
        };
    }

    private void OpenProperLobbyWindow(LobbyType lobbyType)
    {
        for (int i = 0; i < lobbyTypes.Length; i++)
        {
            TurnCanvasGroup(lobbyTypes[i], false);
        }
        TurnCanvasGroup(lobbyType, true);
    }

    private void TurnCanvasGroup(LobbyType lobbyType ,bool turnOn)
    {
        CanvasGroup canvasGroup = lobbyType.GetComponent<CanvasGroup>();
        canvasGroup.alpha = turnOn ? 1 : 0;
        canvasGroup.interactable = turnOn;
        canvasGroup.blocksRaycasts = turnOn;
    }

}
