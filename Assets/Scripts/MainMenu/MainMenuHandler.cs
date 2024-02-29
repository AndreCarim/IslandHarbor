using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuHandler : NetworkBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

   

    private const int MAX_PLAYER_AMOUNT = 4;

    private void Awake(){


        createGameButton.onClick.AddListener(() => {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
        });

        joinGameButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();//start the client
        });
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
