using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class WariningUI : MonoBehaviour
{
    [SerializeField] private Transform WarningUI;
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField] private Button closeButton;

    private void Awake(){
        closeButton.onClick.AddListener(hide);
    } 

    private void Start(){
        LobbyHandler.Instance.OnFailedToJoinGame += MainMenuHandler_OnFailedToJoinGame;

        hide();
    }

    private void MainMenuHandler_OnFailedToJoinGame(object sender, System.EventArgs e){
        if(!warningText)return;

        Show();
       

        warningText.text = NetworkManager.Singleton.DisconnectReason;

        if(warningText.text == ""){
            //connection failed to connect
            warningText.text = "Failed to connect";
        }
    }

    private void Show(){
        WarningUI.gameObject.SetActive(true);
    }

    private void hide(){
        if(!warningText)return;

        WarningUI.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        LobbyHandler.Instance.OnFailedToJoinGame -= MainMenuHandler_OnFailedToJoinGame;
    }
}
