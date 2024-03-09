using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class LobbyHandler : NetworkBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private const int MAX_PLAYER_AMOUNT = 4;

    public static LobbyHandler Instance{get; private set;}

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;


    private void Awake(){
        
        Instance = this;

        createGameButton.onClick.AddListener(() => {
            //start the Host
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetorkManager_Server_OnClientDiscconectCallback;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
        });

        joinGameButton.onClick.AddListener(() => {
            //start the client
            OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

            NetworkManager.Singleton.OnClientDisconnectCallback += NetorkManager_Client_OnClientDiscconectCallback;
            NetworkManager.Singleton.StartClient();
        });
    } 

    private void NetworkManager_OnClientConnectedCallback(ulong clientId){
        ClientsHandler.Instance.updateList(clientId);
    }

    private void NetorkManager_Server_OnClientDiscconectCallback(ulong clientId){
        //this is only available for the server
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsListening) {
            ClientsHandler.Instance.removeFromList(clientId);
        }
        
    }    

    private void NetorkManager_Client_OnClientDiscconectCallback(ulong clientId){
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);   
    }


    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response){
        //this will check if the amount of players is less than the permited

        if(NetworkManager.Singleton.ConnectedClientsIds.Count == 0){
            //the player is trying to host a game
            response.Approved = true;
            return;
        }
        
        if(SceneManager.GetActiveScene().name != "CharacterSelect"){
            //the host is no longer on the characterselect scene, therefore the client cant join
            response.Approved = false;
            response.Reason = "Game has already started";
            return;
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT){
            //the max amount of player was reached
            response.Approved = false;
            response.Reason = "Game is full";

            return;
        }
        
       
        //there is room in the game and game has not started yet
        response.Approved = true;
    }

}
