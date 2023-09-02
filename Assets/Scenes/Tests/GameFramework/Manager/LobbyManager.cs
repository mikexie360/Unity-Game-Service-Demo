using GameFramework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameFramework.Manager
{
    public class LobbyManager : MySingleton<LobbyManager>
    {
        private Lobby _lobby;
        private Coroutine _heatbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;


        public async Task<bool> CreateLobby(int maxPlayers,bool isPrivate, Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);

            }catch (Exception ex)
            {
                Debug.Log("Failed to create lobby");
                return false;
            }

            Debug.Log($"Lobby created with lobby id: {_lobby.Id}");

            _heatbeatCoroutine = StartCoroutine(HeatbeatLobbyCoroutine(_lobby.Id,6f));
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));

            return true;
        }

        private IEnumerator HeatbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log("Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSeconds(waitTimeSeconds);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log("Refresh");
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                Lobby newLobby = task.Result;
                if(newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                }
                yield return new WaitForSeconds(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            foreach(var(key,value)in data)
            {
                playerData.Add(key, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
                    value: value)
                );
            }
            return playerData;
        }
        public void OnApplicationQuite()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync( _lobby.Id );
            }
        }

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
        }
    }


}

