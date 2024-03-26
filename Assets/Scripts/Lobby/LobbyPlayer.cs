using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private Image _isReadyIcon;

    private LobbyPlayerData _data;


    public void SetData(LobbyPlayerData data)
    {
        _data = data;
        _playerName.text = _data.GamerTag;

        if (_data.IsReady)
        {
            _isReadyIcon.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }
}
