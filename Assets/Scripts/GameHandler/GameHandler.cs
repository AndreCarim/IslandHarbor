using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;


public class GameHandler : NetworkBehaviour
{
   [SerializeField] private GameObject playerPrefab;
   public override void OnNetworkSpawn(){
        if(IsServer){
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
   }

   private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut){
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
            GameObject playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
   }
}
