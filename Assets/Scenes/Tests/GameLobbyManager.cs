using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using GameFramework.Core;
using GameFramework.Manager;
using System.Threading.Tasks;
using System;

public class GameLobbyManager : MySingleton<GameLobbyManager>
{
    public async Task<bool> CreateLobby()
    {
        Dictionary<string, string> playerData = new Dictionary<string, string>()
        {
            {"GamerTag", "HostPlayer" }
        };
        bool succeeded = await LobbyManager.Instance.CreateLobby(4, true, playerData);
        return succeeded;
    }

    public string GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }
}
