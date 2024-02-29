using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class CharacterSelectScript : NetworkBehaviour
{
    [SerializeField] private Button readyButton;

    
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake(){
        playerReadyDictionary = new Dictionary<ulong, bool>();

        readyButton.onClick.AddListener(() => {
            SetPlayerReadyServerRpc();
        }); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]){
                //this player is not ready
                allClientsReady = false;
                break;
            }
        }

        if(allClientsReady){
            NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }
}
