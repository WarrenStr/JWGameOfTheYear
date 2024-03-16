using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyUI;
    [SerializeField] private Button _readyButton;


    private void OnEnable()
    {
        _readyButton.onClick.AddListener(OnReadyPressed);
    }

    private void OnDisable()
    {
        _readyButton.onClick.RemoveAllListeners();
    }

    void Start()
    {
        _lobbyUI.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
    }


    void Update()
    {
        
    }

    private async void OnReadyPressed()
    {
        bool succeed = await GameLobbyManager.Instance.SetPlayerReady();

        if(succeed)
        {
            _readyButton.gameObject.SetActive(false);
        }
    }
}
