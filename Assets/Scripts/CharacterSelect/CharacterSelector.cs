using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int clientIndex;
    [SerializeField] private GameObject readySign;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshProUGUI playerName;

    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = TerraNovaManager.Instance.getPlayerDataFromClientIndex(clientIndex);

            TerraNovaManager.Instance.kickPlayer(playerData.clientId);
        });

    }

    private void Start(){
        TerraNovaManager.Instance.OnPlayerDataNetworkListChange += Instance_OnPlayerDataNetworkListChanged;
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

    public void updatePlayer(){
        if(TerraNovaManager.Instance.IsClientIndexConnected(clientIndex)){
            show();
            PlayerData playerData = TerraNovaManager.Instance.getPlayerDataFromClientIndex(clientIndex);
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
        TerraNovaManager.Instance.OnPlayerDataNetworkListChange -= Instance_OnPlayerDataNetworkListChanged;
    }

}
