using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Networking.Transport.Relay;


public class TerraNovaManager : NetworkBehaviour
{
   private const int MAX_PLAYER_AMOUNT = 4;

    public static TerraNovaManager Instance{get; private set;}

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;   
    public event EventHandler OnCreatingLobby;
    public event EventHandler OnPlayerDataNetworkListChange; // Event triggered when player data network list changes

    private NetworkList<PlayerData> playerDataNetworkList; // NetworkList to store player data

    private bool isServerShuttingDown = false;

    private string relayCode;


    private void Awake(){
        DontDestroyOnLoad(gameObject); // Prevents destruction of this object when scene changes
        Instance = this;

        initializeUnityAuthentication();

        playerDataNetworkList = new NetworkList<PlayerData>(); // Initializing player data network list
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged; // Subscribing to list changed eve
    }

 

    public async void startHost(){
        OnCreatingLobby?.Invoke(this, EventArgs.Empty);
        try{
            //start the alocation to the relay
            Allocation allocation = await allocateRelay();

            relayCode = await getRelayJoinCode(allocation);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            //start the alocation to the relay

            //start the Host
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetorkManager_Server_OnClientDiscconectCallback;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
            //start the Host
        }catch{
            //show error window
        }
        
    }

    public  async void startClient(string clientCode){
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        try{
            //join Relay
            JoinAllocation joinAllocation = await joinRelay(clientCode);
            relayCode = clientCode.ToUpper();

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            //start the client
            NetworkManager.Singleton.OnClientDisconnectCallback += NetorkManager_Client_OnClientDiscconectCallback;
            NetworkManager.Singleton.StartClient();
        }catch(Exception e){
            // Handle other exceptions
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
        }
        
    }

    private async Task<JoinAllocation> joinRelay(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }

    private async Task<Allocation> allocateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER_AMOUNT - 1);

            return allocation;
        }catch(RelayServiceException e){
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> getRelayJoinCode(Allocation allocation){
        try{
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }catch(RelayServiceException e){
            Debug.Log(e);

            return default;
        }
    }

    // Event handler for player data network list changes
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent){
        OnPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty); // Triggering the OnPlayerDataNetworkListChange event
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId){
        playerDataNetworkList.Add(new PlayerData{
            clientId = clientId,
        });
    }

    private void NetorkManager_Server_OnClientDiscconectCallback(ulong clientId){
        //this is only available for the server

        if(isServerShuttingDown)return;//if the server is shutting down, do nothing
        //this is so to prevent errors while the server is shutting down and the list
        removeFromList(clientId);
       
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

    public void removeFromList(ulong clientId){
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

    private async void initializeUnityAuthentication(){
        if(UnityServices.State != ServicesInitializationState.Initialized){  
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public string getRelayCode(){
        return relayCode;
    }
}
