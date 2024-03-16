using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
    private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();

    private LobbyPlayerData _localLobbyPlayerData;


    private void OnEnable()
    {
        LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }


    private void OnDisable()
    {
        LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }


    public async Task<bool> CreateLobby()
    {
        LobbyPlayerData playerData = new LobbyPlayerData();
        playerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "Host Player");   

        bool succeeded = await LobbyManager.Instance.CreateLobby(maxPlayers: 4, isPrivate: true, playerData.Serialize());
        return succeeded;
    }


    public String GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }


    public async Task<bool> JoinLobby(string code)
    {
        LobbyPlayerData playerData = new LobbyPlayerData();
        playerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "Join Player");

        bool succeeded = await LobbyManager.Instance.JoinLobby(code, playerData.Serialize());
        return succeeded;
    }


    private void OnLobbyUpdated(Lobby lobby)
    {
        List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayersData();

        _lobbyPlayerDatas.Clear();

        foreach (Dictionary<string, PlayerDataObject> data in playerData) 
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.Initialize(data);

            if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId) //Local User
            {
                _localLobbyPlayerData = lobbyPlayerData;
                //Debug.Log("Local User Found " + lobbyPlayerData.GamerTag + "  IsReady:" + lobbyPlayerData.IsReady);
            }

            _lobbyPlayerDatas.Add(lobbyPlayerData);

        }

        LobbyEvents1.OnLobbyUpdated?.Invoke();
    }


    public List<LobbyPlayerData> GetPlayers()
    {
        return _lobbyPlayerDatas;
    }


    public async Task<bool> SetPlayerReady()
    {
        _localLobbyPlayerData.IsReady = true;

        return await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
    }
}
