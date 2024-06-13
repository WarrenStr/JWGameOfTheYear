// Need more info on why this is implemented https://www.youtube.com/watch?v=226RNT7T9Pw&t=8s
// This is the one that is in the game.events namespace from the video

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

//We want a specific part of the game to come and fetch the data into the gamelobby manager

public static class LobbyEvents // Stores delegates so we can throw them. 
{
    public delegate void LobbyUpdated(Lobby lobby);
    public static LobbyUpdated OnLobbyUpdated;
}
