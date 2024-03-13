using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
public class LobbyButtonshandler : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;    
    [SerializeField] private GameObject relayCodeWindow;
    [SerializeField] private Button verifyCodeButton;
    [SerializeField] private TMP_InputField inputCode;
    [SerializeField] private Button returnButtton;

    private void Awake() {
        hostButton.onClick.AddListener(() => {
            TerraNovaManager.Instance.startHost();
        });

        joinButton.onClick.AddListener(showRelayWindow);

        verifyCodeButton.onClick.AddListener(() => {
            if(inputCode.text.Length > 0){
                TerraNovaManager.Instance.startClient(inputCode.text);
            }        
        });

        returnButtton.onClick.AddListener(hideRelayWindow);
    }

    private void showRelayWindow(){
        relayCodeWindow.SetActive(true);
    }

    private void hideRelayWindow(){
        relayCodeWindow.SetActive(false);
    }
 
}
