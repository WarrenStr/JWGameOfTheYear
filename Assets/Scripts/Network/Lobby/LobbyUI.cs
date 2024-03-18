using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyUI;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Image _mapImage;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TextMeshProUGUI _mapName;
    [SerializeField] private MapSelectionData _mapSelectionData;

    private int _currentMapIndex = 0;


    private void OnEnable()
    {
        _readyButton.onClick.AddListener(OnReadyPressed);
        _leftButton.onClick.AddListener(OnLeftButtonClick);
        _rightButton.onClick.AddListener(OnRightButtonClick);
    }

    private void OnDisable()
    {
        _readyButton.onClick.RemoveAllListeners();
        _leftButton.onClick.RemoveAllListeners();
        _rightButton.onClick.RemoveAllListeners();
    }

    void Start()
    {
        _lobbyUI.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
    }

    
    private void OnLeftButtonClick()
    {
        if (_currentMapIndex - 1 >= 0) 
        {
            _currentMapIndex--;
        }
        else
        {
            _currentMapIndex = _mapSelectionData.Maps.Count - 1; //Make it so it wraps around 
        }

        UpdateMap();
    }


    private void OnRightButtonClick()
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
}
