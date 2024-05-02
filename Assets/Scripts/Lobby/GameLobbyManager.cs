using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
    private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
    private LobbyPlayerData _localLobbyPlayerData;
    private LobbyData _lobbyData;
    private int _maxNumberOfPlayers = 4;

    public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.Instance.GetHostId();


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
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "Host Player");

        _lobbyData = new LobbyData();
        _lobbyData.Initialize(mapIndex: 0);

        bool succeeded = await LobbyManager.Instance.CreateLobby(_maxNumberOfPlayers, isPrivate: true,data: _localLobbyPlayerData.Serialize(), _lobbyData.Serialize());
        return succeeded;
    }


    public String GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }


    public async Task<bool> JoinLobby(string code)
    {
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "Join Player");

        bool succeeded = await LobbyManager.Instance.JoinLobby(code, _localLobbyPlayerData.Serialize());
        return succeeded;
    }


    private async void OnLobbyUpdated(Lobby lobby)
    {
        List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayersData();

        _lobbyPlayerDatas.Clear();


        int numberOfPlayersReady = 0;
        foreach (Dictionary<string, PlayerDataObject> data in playerData) 
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.Initialize(data);

            if (lobbyPlayerData.IsReady)
            {
                numberOfPlayersReady++;
            }

            if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId) //Local User
            {
                _localLobbyPlayerData = lobbyPlayerData;
            }

            _lobbyPlayerDatas.Add(lobbyPlayerData);

        }

        _lobbyData = new LobbyData();
        _lobbyData.Initialize(lobby.Data);

        LobbyEvents1.OnLobbyUpdated?.Invoke();

        if(numberOfPlayersReady == lobby.Players.Count) 
        {
            LobbyEvents1.OnLobbyReady?.Invoke();
        }

        if (_lobbyData.RelayJoinCode != default) 
        {
            await JoinRelayServer(_lobbyData.RelayJoinCode);
            SceneManager.LoadSceneAsync(_lobbyData.SceneName);
        }
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


    public int GetMapIndex()
    {
        return _lobbyData.MapIndex;
    }


    public async Task<bool> SetSelectedMap(int currentMapIndex, string sceneName)
    {
        _lobbyData.MapIndex = currentMapIndex;
        _lobbyData.SceneName = sceneName;
        return await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize()); 
    }


    public async Task StartGame()
    {
        string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxNumberOfPlayers);

        _lobbyData.RelayJoinCode = relayJoinCode;
        await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

        string allocationId = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();
        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

        SceneManager.LoadSceneAsync(_lobbyData.SceneName);
    }

    private async Task<bool> JoinRelayServer(string relayJoinCode)
    {
        await RelayManager.Instance.JoinRelay(relayJoinCode);
        string allocationId = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();
        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);
        return true;
    }
}
