using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private GameObject _joinScreen;

    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;

    [SerializeField] private Button _submitCodeButton;

    [SerializeField] private TextMeshProUGUI _codeText;


    private void OnEnable()
    {
        _hostButton.onClick.AddListener(OnHostClicked);
        _joinButton.onClick.AddListener(OnJoinClicked);
        _submitCodeButton.onClick.AddListener(OnSubmitCodeClicked);
    }


    private async void OnHostClicked()
    {
        //Debug.Log(message: "Host");
        bool succeedded = await GameLobbyManager.Instance.CreateLobby();
        if (succeedded) 
        {
            SceneManager.LoadSceneAsync("Lobby");
        }
    }


    private void OnJoinClicked()
    {
        //Debug.Log(message: "Join");
        _mainScreen.SetActive(false);
        _joinScreen.SetActive(true);
    }


    private async void OnSubmitCodeClicked()
    {
        string code = _codeText.text;
        code = code.Substring(0, code.Length - 1); //TextMeshPro adds a end of line character to the end of string that we need to remove
        //Debug.Log(code);

        bool succeeded = await GameLobbyManager.Instance.JoinLobby(code);
        
        if (succeeded)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }     
    }


    private void OnDisable()
    {
        _hostButton.onClick.RemoveListener(OnHostClicked);
        _joinButton.onClick.RemoveListener(OnJoinClicked);
        _submitCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);
    }
}
