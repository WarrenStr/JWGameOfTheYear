using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
    public async Task<bool> CreateLobby()
    {
        Dictionary<string, string> playerData = new Dictionary<string, string>()
        {
            {"GamerTag", "HostPlayer" }
        };

        bool succeeded = await LobbyManager.Instance.CreateLobby(maxPlayers: 4, isPrivate: true, playerData);
        return succeeded;
    }


    public String GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }


    public async Task<bool> JoinLobby(string code)
    {
        Dictionary<string, string> playerData = new Dictionary<string, string>()
        {
            {"GamerTag", "Join Player" }
        };

        bool succeeded = await LobbyManager.Instance.JoinLobby(code, playerData);
        return succeeded;
    }
}
