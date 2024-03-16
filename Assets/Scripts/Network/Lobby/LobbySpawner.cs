using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySpawner : MonoBehaviour
{
    [SerializeField] private List<LobbyPlayer> _players;

    private void OnEnable()
    {
        LobbyEvents1.OnLobbyUpdated += OnLobbyUpdated;
    }


    private void OnDisable()
    {
        LobbyEvents1.OnLobbyUpdated -= OnLobbyUpdated;
    }


    private void OnLobbyUpdated()
    {
        List<LobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers(); //Make sure player data is synced in order for all players

        for (int i = 0; i < playerDatas.Count; i++) 
        {
            LobbyPlayerData data = playerDatas[i];
            _players[i].SetData(data);
        }
    }
}
