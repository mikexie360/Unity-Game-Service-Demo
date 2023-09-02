using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;
    // Start is called before the first frame update
    void Start()
    {
        _hostButton.onClick.AddListener(OnHostClicked);
        _joinButton.onClick.AddListener(OnJoinClicked);
    }

    private async void OnHostClicked()
    {
        bool succeeded = await GameLobbyManager.Instance.CreateLobby();

        if (succeeded)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }
    }
    private void OnJoinClicked()
    {
        Debug.Log("Join");

    }
}
