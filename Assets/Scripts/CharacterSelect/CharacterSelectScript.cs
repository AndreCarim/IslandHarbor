using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class CharacterSelectScript : NetworkBehaviour
{


    [SerializeField] private Button readyButton; // Serialized field for a UI button to indicate readiness
    [SerializeField] private Button mainMenuButton; // Serialized field for a UI button to return to the main menu

    private Dictionary<ulong, bool> playerReadyDictionary; // Dictionary to store player readiness status, with Client ID as key and readiness status as value

    
    
    private void Awake(){ // Awake method is called when the script instance is being loaded


        playerReadyDictionary = new Dictionary<ulong, bool>(); // Initializing the playerReadyDictionary

        // Adding a listener to the readyButton click event to call SetPlayerReadyServerRpc method
        readyButton.onClick.AddListener(() => {
            SetPlayerReadyServerRpc();
        }); 

        // Adding a listener to the mainMenuButton click event to shut down the network and return to the main menu
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown(); // Shutting down the network
            if(NetworkManager.Singleton != null){ // Checking if the NetworkManager instance exists
                Destroy(NetworkManager.Singleton.gameObject); // Destroying the NetworkManager instance
            }
            SceneManager.LoadScene("MainMenu"); // Loading the main menu scene
        });

        
    }

    [ServerRpc(RequireOwnership = false)] // Declaring a ServerRpc method
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){ // Method to handle setting player readiness on the server
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

    

    
}
