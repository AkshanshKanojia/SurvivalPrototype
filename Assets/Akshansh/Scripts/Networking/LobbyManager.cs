using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;

public class LobbyManager : MonoBehaviour
{
    public Lobby activeLobby;
    public string PlayerName = "Batman";
    private async void Start()
    {
        //initialize services then sign in 
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    IEnumerator KeepLobbyAlive()
    {
        yield return new WaitForSeconds(25);
        SendHeartBeat();
        StartCoroutine(KeepLobbyAlive());//loop throughout session once activated
    }

    async void SendHeartBeat()
    {
        await LobbyService.Instance.SendHeartbeatPingAsync(activeLobby.Id);
    }

    public async void CreateLobby(string _lobbyName, int _playerCount)
    {
        try
        {
            CreateLobbyOptions _option = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>()
                {{"PlayerName",new DataObject(DataObject.VisibilityOptions.Member,PlayerName,DataObject.IndexOptions.S1)} }
            };
            activeLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _playerCount);
            //join after creating
            if (activeLobby != null)
            {
                print("Join Code: " + activeLobby.LobbyCode);
                FindObjectOfType<NetworkManager>().StartServer();
            }
            //avoid deactivation of lobby dicoverability after creation.
            StartCoroutine(KeepLobbyAlive());

        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

    public async void JoinLobby(string _lobbyCode)
    {
        try
        {
            activeLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(_lobbyCode);
            //do something after joining
            print("Joined lobby " + activeLobby.Players.Count);
            FindObjectOfType<NetworkManager>().StartHost();
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
        
    }
}
