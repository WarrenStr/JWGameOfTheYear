using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The GameLobbyManager class is a singleton that manages the game's lobby system, including creating, joining, and updating lobbies. It handles player data and synchronization across the lobby, manages the state of the game
/// lobby, and transitions between game scenes. This class relies on the Unity Services SDK for Lobby and Relay management
/// </summary>
public class GameLobbyManager : Singleton<GameLobbyManager>
{
    private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
    private LobbyPlayerData _localLobbyPlayerData;
    private LobbyData _lobbyData;
    private int _maxNumberOfPlayers = 4;
    private bool _inGame = false;

    public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.Instance.GetHostId(); // If the local player is the host


    private void OnEnable()
    {
        LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }


    private void OnDisable()
    {
        LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }


    /// <summary>
    /// Asynchronously creates a new lobby and initializes local player data as the host.
    /// </summary>
    /// <returns>A task that returns true if the lobby was created successfully, otherwise false.</returns>
    public async Task<bool> CreateLobby()
    {
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "Host Player");

        _lobbyData = new LobbyData();
        _lobbyData.Initialize(mapIndex: 0);

        bool succeeded = await LobbyManager.Instance.CreateLobby(_maxNumberOfPlayers, isPrivate: true,data: _localLobbyPlayerData.Serialize(), _lobbyData.Serialize());
        return succeeded;
    }


    /// <summary>
    /// Retrieves the code for the current lobby.
    /// </summary>
    /// <returns>The lobby code as a string.</returns>
    public String GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
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

        _lobbyPlayerDatas.Clear(); //Clear the previous data. 


        int numberOfPlayersReady = 0;
        foreach (Dictionary<string, PlayerDataObject> data in playerData) 
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.Initialize(data);

            if (lobbyPlayerData.IsReady)
            {
                numberOfPlayersReady++;
            }

            if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId) //Local user.
            {
                _localLobbyPlayerData = lobbyPlayerData;
            }

            _lobbyPlayerDatas.Add(lobbyPlayerData);
        }

        _lobbyData = new LobbyData();
        _lobbyData.Initialize(lobby.Data); // TO-DO The intial map scene and index should be set somewhere around here. Debugs show the index starts at 0 but the scene name is null

        LobbyEvents1.OnLobbyUpdated?.Invoke();

        if(numberOfPlayersReady == lobby.Players.Count) 
        {
            LobbyEvents1.OnLobbyReady?.Invoke();
        }

        if (_lobbyData.RelayJoinCode != default && !_inGame) 
        {
            await JoinRelayServer(_lobbyData.RelayJoinCode);
            SceneManager.LoadSceneAsync(_lobbyData.SceneName);
        }
    }


    /// <summary>
    /// Retrieves the list of players in the lobby.
    /// </summary>
    /// <returns>A list of LobbyPlayerData objects representing the players in the lobby.</returns>
    public List<LobbyPlayerData> GetPlayers()
    {
        return _lobbyPlayerDatas;
    }


    /// <summary>
    /// Sets the local player as ready and updates the lobby data.
    /// </summary>
    /// <returns>A task that returns true if the player data was successfully updated, otherwise false.</returns>
    public async Task<bool> SetPlayerReady()
    {
        _localLobbyPlayerData.IsReady = true;

        return await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
    }


    /// <summary>
    /// Retrieves the index of the currently selected map.
    /// </summary>
    /// <returns>The index of the currently selected map.</returns>
    public int GetMapIndex()
    {
        return _lobbyData.MapIndex;
    }


    /// <summary>
    /// Sets the selected map for the lobby and updates the lobby data.
    /// </summary>
    /// <param name="currentMapIndex">The index of the selected map.</param>
    /// <param name="sceneName">The name of the scene associated with the selected map.</param>
    /// <returns>A task that returns true if the lobby data was successfully updated, otherwise false.</returns>
    public async Task<bool> SetSelectedMap(int currentMapIndex, string sceneName)
    {
        _lobbyData.MapIndex = currentMapIndex;
        _lobbyData.SceneName = sceneName;

        return await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());   
    }


    /// <summary>
    /// Starts the game by creating a relay, updating the lobby data, and loading the game scene.
    /// </summary>
    public async Task StartGame()
    {
        string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxNumberOfPlayers);
        _inGame = true;

        _lobbyData.RelayJoinCode = relayJoinCode;

        await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

        string allocationId = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();
        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

        SceneManager.LoadSceneAsync(_lobbyData.SceneName);
    }


    private async Task<bool> JoinRelayServer(string relayJoinCode)
    {
        _inGame = true;
        await RelayManager.Instance.JoinRelay(relayJoinCode);
        string allocationId = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();
        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);
        return true;
    }
}
