using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private Image _mapImage;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TextMeshProUGUI _mapName;
    [SerializeField] private MapSelectionData _mapSelectionData;

    public int _currentMapIndex = 0;


    private void OnEnable()
    {
        _readyButton.onClick.AddListener(OnReadyPressed);

        if (GameLobbyManager.Instance.IsHost)
        {
            _readyButton.onClick.AddListener(OnReadyPressed);
            _leftButton.onClick.AddListener(OnLeftButtonClick);
            _rightButton.onClick.AddListener(OnRightButtonClick);
            _startButton.onClick.AddListener(OnStartButtonClicked);

            LobbyEvents1.OnLobbyReady += OnLobbyReady;
        }

        LobbyEvents1.OnLobbyUpdated += OnLobbyUpdated;
    }


    private void OnDisable()
    {
        _readyButton.onClick.RemoveAllListeners();
        _leftButton.onClick.RemoveAllListeners();
        _rightButton.onClick.RemoveAllListeners();
        _startButton.onClick.RemoveAllListeners();

        LobbyEvents1.OnLobbyUpdated -= OnLobbyUpdated;
        LobbyEvents1.OnLobbyReady -= OnLobbyReady;
    }


    async void Start()
    {
        _lobbyCodeText.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";

        if(!GameLobbyManager.Instance.IsHost) // Disable UI elements if you are not the host.
        {
            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
        }
        else
        {
            await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
        }
    }

    
    private async void OnLeftButtonClick()
    {
        if (_currentMapIndex - 1 >= 0) 
        {
            _currentMapIndex--;
        }
        else
        {
            _currentMapIndex = _mapSelectionData.Maps.Count - 1; // Make it so it wraps around.
        }

        UpdateMap();
        await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
    }


    private async void OnRightButtonClick()
    {
        if (_currentMapIndex + 1 <= _mapSelectionData.Maps.Count - 1)
        {
            _currentMapIndex++;
        }
        else
        {
            _currentMapIndex = 0;
        }

        UpdateMap();
        await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
    }


    private async void OnReadyPressed()
    {
        bool succeed = await GameLobbyManager.Instance.SetPlayerReady();

        if(succeed)
        {
            _readyButton.gameObject.SetActive(false);
        }
    }


    private void UpdateMap()
    {
        _mapImage.sprite = _mapSelectionData.Maps[_currentMapIndex].MapThumbnail;
        _mapName.text = _mapSelectionData.Maps[_currentMapIndex].MapName;
    }


    private void OnLobbyUpdated()
    {
        _currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
        UpdateMap();
    }


    private void OnLobbyReady()
    {
        _startButton.gameObject.SetActive(true);
    }


    private async void OnStartButtonClicked()
    {
        await GameLobbyManager.Instance.StartGame();
    }
}
