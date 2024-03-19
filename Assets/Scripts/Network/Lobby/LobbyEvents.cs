using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public static class LobbyEvents
{
    public delegate void LobbyUpdated(Lobby lobby);
    public static LobbyUpdated OnLobbyUpdated;
}
