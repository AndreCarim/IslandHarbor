using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System;
public class PlayerReadyHandler : NetworkBehaviour
{
    public static PlayerReadyHandler Instance{get; private set;}
    [SerializeField] private Button readyButton; // Serialized field for a UI button to indicate readiness
    [SerializeField] private Button mainMenuButton; // Serialized field for a UI button to return to the main menu

    public event EventHandler OnReadyChange;
    private Dictionary<ulong, bool> playerReadyDictionary; // Dictionary to store player readiness status, with Client ID as key and readiness status as value


    
    private void Awake(){ // Awake method is called when the script instance is being loaded

        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>(); // Initializing the playerReadyDictionary

        // Adding a listener to the readyButton click event to call SetPlayerReadyServerRpc method
        readyButton.onClick.AddListener(() => {
            SetPlayerReadyServerRpc();

            readyButton.gameObject.SetActive(false);
        }); 

        // Adding a listener to the mainMenuButton click event to shut down the network and return to the main menu
        mainMenuButton.onClick.AddListener(() => {
            TerraNovaManager.Instance.serverIsShuttingDown();//make sure it does nothing if the server is shutting down

            NetworkManager.Singleton.Shutdown(); // Shutting down the network
            
            SceneManager.LoadScene("MainMenu"); // Loading the main menu scene
        });

       
        TerraNovaManager.Instance.OnPlayerDataNetworkListChange += Instance_OnPlayerDataNetworkListChanged;
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e){
        //this will be called when a player joins or get out of the room
        //so all the players need to press ready again. 
        clearDictionaryServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void clearDictionaryServerRpc(ServerRpcParams serverRpcParams = default){
        playerReadyDictionary.Clear();

        clearDictionaryOnClientRpc();
    }

    [ClientRpc]
    private void clearDictionaryOnClientRpc(){
        playerReadyDictionary.Clear();

        readyButton.gameObject.SetActive(true);
    }



    [ServerRpc(RequireOwnership = false)] // Declaring a ServerRpc method
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){ // Method to handle setting player readiness on the server
        setPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true; // Setting the player readiness status for the sender client to true

        
        bool allClientsReady = true; // Assuming all clients are ready initially
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){ // Iterating through connected client IDs
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]){
                // If the player readiness dictionary doesn't contain the client ID or the client is not ready
                allClientsReady = false; // Set allClientsReady to false
                break; // Exit the loop
            }
        }

        if(allClientsReady){ // If all clients are ready
            NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single); // Load the main scene
        }
    }

    [ClientRpc]
    private void setPlayerReadyClientRpc(ulong clientId){
        playerReadyDictionary[clientId] = true;

        OnReadyChange?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId){
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
     public override void OnDestroy()
    {
        base.OnDestroy(); // Call the base class implementation
        TerraNovaManager.Instance.OnPlayerDataNetworkListChange -= Instance_OnPlayerDataNetworkListChanged;
    }
   
}
