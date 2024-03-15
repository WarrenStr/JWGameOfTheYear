using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyUI;

    void Start()
    {
        _lobbyUI.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
    }


    void Update()
    {
        
    }
}
