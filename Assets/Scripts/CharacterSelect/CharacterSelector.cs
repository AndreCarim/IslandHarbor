using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int clientIndex;
    [SerializeField] private GameObject readySign;
    [SerializeField] private Button kickButton;

    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = ClientsHandler.Instance.getPlayerDataFromClientIndex(clientIndex);

            ClientsHandler.Instance.kickPlayer(playerData.clientId);
        });
    }

    private void Start(){
        ClientsHandler.Instance.OnPlayerDataNetworkListChange += Instance_OnPlayerDataNetworkListChanged;
        PlayerReadyHandler.Instance.OnReadyChange += PlayerReadyHandler_OnReadyChange;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        updatePlayer();
    }

    private void PlayerReadyHandler_OnReadyChange(object sender, System.EventArgs e){
        updatePlayer();
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e){
        updatePlayer();
    }

    private void updatePlayer(){
        if(ClientsHandler.Instance.IsClientIndexConnected(clientIndex)){
            show();
            PlayerData playerData = ClientsHandler.Instance.getPlayerDataFromClientIndex(clientIndex);
            readySign.SetActive(PlayerReadyHandler.Instance.IsPlayerReady(playerData.clientId));
        }else{
            hide();
        }
    }

    private void show(){
        if(gameObject != null){
            gameObject.SetActive(true);
        }
    }

    private void hide(){
        if(gameObject != null){
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy() {
        ClientsHandler.Instance.OnPlayerDataNetworkListChange -= Instance_OnPlayerDataNetworkListChanged;
    }
}
