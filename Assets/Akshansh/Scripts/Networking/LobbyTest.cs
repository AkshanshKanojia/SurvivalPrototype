using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTest : MonoBehaviour
{
    public string lobbyName = "Lobby", joinCode, PlayerName = "Batman";
    public int maxPlayers = 2;
    public bool isPrivate = false;

    [SerializeField] Button CreateButt, JoinButt;

    Lobby hostLobby;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        CreateButt.onClick.AddListener(CreateLobby);
        JoinButt.onClick.AddListener(JoinLobby);
    }
    IEnumerator KeepLobbyAlive()
    {
        yield return new WaitForSeconds(20);
        if (hostLobby != null)
        {
            SendHeartBeat();
        }
        StartCoroutine(KeepLobbyAlive());//kill this when connection closed
    }
    async void SendHeartBeat()
    {
        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        print("beat");
    }
    async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions _options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = GetPlayer()
            };
            Lobby _lob = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, _options);
            print("lobby generated " + _lob.Name + " " + _lob.LobbyCode);
            hostLobby = _lob;
            PrintPlayer(_lob);
            StartCoroutine(KeepLobbyAlive());
            CreateLobbyList();
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>()
            {
                { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,PlayerName) }
            }
        };
    }
    async void CreateLobbyList()
    {
        try
        {
            QueryLobbiesOptions _options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter> { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) },
                Order = new List<QueryOrder> { new QueryOrder(false, QueryOrder.FieldOptions.Created) }
            };
        }
        catch { }
        QueryResponse _response = await Lobbies.Instance.QueryLobbiesAsync();
        print("Lobbies in scene " + _response.Results.Count);
    }

    async void JoinLobby()
    {
        try
        {
            QueryResponse _response = await Lobbies.Instance.QueryLobbiesAsync();
            JoinLobbyByCodeOptions _op = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            PrintPlayer(await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode,_op));
            
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    void PrintPlayer(Lobby _lob)
    {
        foreach (var v in _lob.Players)
        {
            print("Lobby players" + v.Data["PlayerName"].Value);
        }
    }
}
