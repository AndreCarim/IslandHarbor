using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System;

// This script ensures it persists between scene changes and handles player data network list
public class ClientsHandler : NetworkBehaviour
{
    public static ClientsHandler Instance{get; private set;} // Singleton instance of ClientsHandler

    public event EventHandler OnPlayerDataNetworkListChange; // Event triggered when player data network list changes

    private NetworkList<PlayerData> playerDataNetworkList; // NetworkList to store player data

    private bool isServerShuttingDown = false;
   
    private void Awake(){
        DontDestroyOnLoad(gameObject); // Prevents destruction of this object when scene changes

        Instance = this; // Assigning the current instance to the singleton Instance

        playerDataNetworkList = new NetworkList<PlayerData>(); // Initializing player data network list
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged; // Subscribing to list changed event
    }

    // Event handler for player data network list changes
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent){
        OnPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty); // Triggering the OnPlayerDataNetworkListChange event
    }

    // Method to update player data list with a new client ID
    public void updateList(ulong clientId){   
        playerDataNetworkList.Add(new PlayerData{
            clientId = clientId,
        });
    }

    public void removeFromList(ulong clientId){
        if(isServerShuttingDown)return;//if the server is shutting down, do nothing
        //this is so to prevent errors while the server is shutting down and the list

        // Find the index of the player data with the matching clientId
       // Check if the current instance is running as a server
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer) {
            for(int i = 0; i < playerDataNetworkList.Count; i++){
                PlayerData playerData = playerDataNetworkList[i];
                if(playerData.clientId == clientId){
                    //client that disconnected
                    if(playerDataNetworkList != null && playerDataNetworkList.Contains(playerData) ){
                        playerDataNetworkList.RemoveAt(i);
                    }
                    break;
                }
            }
        }
    }
    

    // Method to check if a client index is connected
    public bool IsClientIndexConnected(int clientIndex){
        return clientIndex < playerDataNetworkList.Count; // Checking if client index is within the player data network list count
    }

    public PlayerData getPlayerDataFromClientIndex(int clientIndex){
        return playerDataNetworkList[clientIndex];
    }

    public void serverIsShuttingDown(){
        if(!IsServer)return;
        isServerShuttingDown = true;
    }

    public void kickPlayer(ulong clientId){
        if(clientId == NetworkManager.ServerClientId){
            //the server is kicking the server
            isServerShuttingDown = true;
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }else{
            NetworkManager.Singleton.DisconnectClient(clientId);
        }


        
        removeFromList(clientId);
    }
}
