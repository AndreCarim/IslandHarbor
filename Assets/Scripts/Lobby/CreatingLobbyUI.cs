using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
public class CreatingLobbyUI : MonoBehaviour
{
    [SerializeField] private Transform creatingLobbyUI;

    private void Start(){
        TerraNovaManager.Instance.OnCreatingLobby += TerraNovaManager_OnCreatingLobby;
    }

    private void TerraNovaManager_OnCreatingLobby(object sender, System.EventArgs e){
        if(!creatingLobbyUI)return;

        show();
    }

    private void show(){
        creatingLobbyUI.gameObject.SetActive(true);
    }

    private void hide(){
        creatingLobbyUI.gameObject.SetActive(false);
    }


    private void OnDestroy() {
        TerraNovaManager.Instance.OnCreatingLobby -= TerraNovaManager_OnCreatingLobby;
    }
}
