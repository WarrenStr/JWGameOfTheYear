using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System;

public class LobbyManager : Singleton<LobbyManager>
{
    private Lobby _lobby;
    private Coroutine _heatbeatCoroutine;
    private Coroutine _refreshLobbyCoroutine;


    internal string GetLobbyCode()
    {
        return _lobby?.LobbyCode;
    }


    public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

        Player player = new Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

        CreateLobbyOptions options = new CreateLobbyOptions()
        {
            IsPrivate = isPrivate,
            Player = player
        };

        try
        {
            _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
        }  
        catch (System.Exception) 
        {
            return false;
        }

        //Debug.Log(message: $"Lobby created with lobby id {_lobby.Id}");

        _heatbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 6.0f)); //Only the host needs to do the heartbeat
        _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1.0f));

        return true;
    }


    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) //Heartbeat
    {
        while (true) 
        {
            //Debug.Log(message: "Heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }


    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds) //Refresh
    {
        while (true)
        {
            //Debug.Log(message: "Lobby Refresh");
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            yield return new WaitUntil(() => task.IsCompleted);
            Lobby newLobby = task.Result;

            if (newLobby.LastUpdated > _lobby.LastUpdated)
            {
                _lobby = newLobby;  
                LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
            }

            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }


    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();

        foreach (var (key, value) in data)  //Dictionar<string,string>
        {

            playerData.Add(key, new PlayerDataObject(
                visibility: PlayerDataObject.VisibilityOptions.Member,
                value: value));
        }
        return playerData;
    }


    public void OnApplicationQuit()
    {
        if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId) //If we are the host
        {
            LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
        }
    }


    public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
    {
        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
        Player player = new Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, SerializePlayerData(playerData));

        options.Player = player;


        try 
        {
            _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
        }
        catch
        {
            return false;
        }


        _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1.0f));
        return true;
    }


    public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
    {
        List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

        foreach (Player player in _lobby.Players)
        {
            data.Add(player.Data);
        }

        return data;
    }
}
