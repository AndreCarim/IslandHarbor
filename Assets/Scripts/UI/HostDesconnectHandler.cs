using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HostDesconnectHandler : MonoBehaviour
{

    [SerializeField] private Transform hostDisconnectedUI;
    [SerializeField] private Button mainMenuButton;

    private void Awake(){
        NetworkManager.Singleton.OnClientDisconnectCallback += NetWorkManager_OnClientDisconnectCallBack;

        mainMenuButton.onClick.AddListener(goToMainMenu);
    }

    private void goToMainMenu(){
        ClientsHandler.Instance.serverIsShuttingDown();

        NetworkManager.Singleton.Shutdown();//shut down the server

        SceneManager.LoadScene("MainMenu");
    }

    private void NetWorkManager_OnClientDisconnectCallBack(ulong clientId){
        if(clientId == NetworkManager.ServerClientId){
            //server is shutting down
            show();
        }
    }

    private void show(){
        hostDisconnectedUI.gameObject.SetActive(true);
    }

    private void hide(){
        hostDisconnectedUI.gameObject.SetActive(false);
    }

    private void OnDisable() {
        if(NetworkManager.Singleton != null){//if the host disconnects, the networkManager will already be destroyed in the scene, so, we just check if the networkmanager is there still.
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetWorkManager_OnClientDisconnectCallBack;
        }  
    }


}
