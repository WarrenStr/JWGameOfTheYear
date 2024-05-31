// Player Prefs https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
// Async https://gamedevbeginner.com/async-in-unity/
// AuthenticationServices https://docs.unity.com/ugs/en-us/manual/authentication/manual/get-started

using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The InitUnityServices class initializes Unity services and handles anonymous user authentication. Upon successful sign-in, it loads the Main Menu scene.
/// </summary>
public class InitUnityServices : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();

        if(UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn; //Events 

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if(AuthenticationService.Instance.IsSignedIn)
            {
                string username = PlayerPrefs.GetString(key: "Username"); 
                if (username == "")
                {
                    username = "Player";
                    PlayerPrefs.SetString("Username", username);
                }

                SceneManager.LoadSceneAsync("Main Menu");
            }
        }
    }

    private void OnSignedIn()
    {
        Debug.Log(message: $"Player Id: {AuthenticationService.Instance.PlayerId}");
        Debug.Log(message: $"Token: {AuthenticationService.Instance.AccessToken}");
    }

    private void OnDisable()
    {
        AuthenticationService.Instance.SignedIn -= OnSignedIn;
    }
}
