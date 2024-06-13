//This class does not utilze a parameter within the declaration of the delegate

using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyEvents1 : MonoBehaviour
{
    public delegate void LobbyUpdated(); // We want game to fetch game lobby info?
    public static LobbyUpdated OnLobbyUpdated;

    public delegate void LobbyReady();
    public static LobbyReady OnLobbyReady;
}
