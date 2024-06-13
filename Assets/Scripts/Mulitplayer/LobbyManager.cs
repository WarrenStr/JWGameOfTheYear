// Create a lobby https://docs.unity.com/ugs/en-us/manual/lobby/manual/create-a-lobby

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System;

/// <summary>
/// The LobbyManager class is a singleton that handles the creation, joining, and management of game lobbies using Unity's Lobby service.
/// It manages the lobby state, player data, and ensures synchronization across players in the lobby.
/// </summary>
public class LobbyManager : Singleton<LobbyManager>
{
    private Lobby _lobby;
    private Coroutine _heatbeatCoroutine;
    private Coroutine _refreshLobbyCoroutine;


    internal string GetLobbyCode()
    {
        return _lobby?.LobbyCode;
    }


    /// <summary>
    /// Creates a new lobby with the specified parameters.
    /// </summary>
    /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
    /// <param name="isPrivate">Indicates whether the lobby is private.</param>
    /// <param name="data">Player data to be serialized and included in the lobby creation.</param>
    /// <param name="lobbyData">Lobby data to be serialized and included in the lobby creation.</param>
    /// <returns>A task that returns true if the lobby was created successfully, otherwise false.</returns>

    public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

        Player player = new (AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

        CreateLobbyOptions options = new CreateLobbyOptions()
        {
            Data = SerializeLobbyData(lobbyData),
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

        Debug.Log(message: $"Lobby created with lobby id {_lobby.Id}");

        _heatbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 6.0f)); // Only the host needs to do the heartbeat.
        _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1.0f));

        return true;
    }


    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) // Heartbeat.
    {
        while (true) 
        {
            Debug.Log(message: "Heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }


    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds) // Refresh.
    {
        while (true)
        {
            Debug.Log(message: "Lobby Refresh");
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

        foreach (var (key, value) in data)  //Dictionary<string,string>
        {

            playerData.Add(key, new PlayerDataObject(
                visibility: PlayerDataObject.VisibilityOptions.Member,
                value: value));
        }
        return playerData;
    }


    private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
        foreach (var(key, value) in data)
        {
            lobbyData.Add(key, new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: value));
        }

        return lobbyData;
    }


    /// <summary>
    /// Cleans up resources when the application quits. If the current player is the host, it deletes the lobby.
    /// </summary>
    public void OnApplicationQuit()
    {
        if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId) // If we are the host.
        {
            LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
        }
    }


    /// <summary>
    /// Joins an existing lobby using the provided  lobby code and player data.
    /// </summary>
    /// <param name="code">The code of the lobby to join.</param>
    /// <param name="playerData">Player data to be serialized and included in the join request.</param>
    /// <returns>A task that returns true if the lobby was joined successfully, otherwise false.</returns>
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

        _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, waitTimeSeconds: 1.0f));
        return true;
    }


    /// <summary>
    /// Retrieves the data for all players in the lobby.
    /// </summary>
    /// <returns>A list of dictionaries containing player data objects for each player in the lobby.</returns>
    public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
    {
        List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

        foreach (Player player in _lobby.Players)
        {
            data.Add(player.Data);
        }

        return data;
    }


    /// <summary>
    /// Updates the data for a specific player in the lobby.
    /// </summary>
    /// <param name="playerId">The ID of the player to update.</param>
    /// <param name="data">The new data for the player.</param>
    /// <param name="allocatioId">The allocation ID for relay connections (optional).</param>
    /// <param name="connectionData">The connection data for relay connections (optional).</param>
    /// <returns>A task that returns true if the player data was updated successfully, otherwise false.</returns>
    public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocatioId = default, string connectionData = default) // Default makes parameter optional. 
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        UpdatePlayerOptions options = new UpdatePlayerOptions()
        {
            Data = playerData,
            AllocationId = allocatioId,
            ConnectionInfo = connectionData
        };

        try
        {
           _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
        }
        catch (System.Exception)
        {
            return false; 
        }

        LobbyEvents.OnLobbyUpdated(_lobby);

        return true;
    }


    /// <summary>
    /// Updates the data for the lobby.
    /// </summary>
    /// <param name="data">The new data for the lobby.</param>
    /// <returns>A task that returns true if the lobby data was updated successfully, otherwise false.</returns>
    public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject>  lobbyData = SerializeLobbyData(data);

        UpdateLobbyOptions options = new UpdateLobbyOptions()
        { 
            Data = lobbyData 
        };

        try
        {
            _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
        }
        catch (System.Exception)
        {
            return false;
        }

        LobbyEvents.OnLobbyUpdated(_lobby);

        return true;
    }


    /// <summary>
    /// Retrieves the ID of the host of the lobby.
    /// </summary>
    /// <returns>The host ID as a string.</returns>
    public string GetHostId()
    {
        return _lobby.HostId;
    }
}
