using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.ClearSessionToken();
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {

            AuthenticationService.Instance.SignedIn += OnSignedIn;

            await AuthenticationService.Instance.SignInAnonymouslyAsync();


            if (AuthenticationService.Instance.IsSignedIn)
            {
                string username = PlayerPrefs.GetString("Username");
                if (username == "")
                {
                    username = "Player";
                    PlayerPrefs.SetString("Username", username);
                }
                Debug.Log("Signed in Anonymously");

                SceneManager.LoadSceneAsync("Main Menu");
            }
        }
    }

    private void OnSignedIn()
    {
        Debug.Log($"Access token: {AuthenticationService.Instance.AccessToken}");
        Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
