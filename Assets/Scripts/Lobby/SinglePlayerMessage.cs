using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerMessage : MonoBehaviour
{
    [SerializeField] private GameObject singlePlayerMessage;

    private void Start() {
        if(TerraNovaManager.playMultiplayer == false){
            //means that its singleplayer
            show();
        }else{
            //multiplayer
            hide();
        }
    }

    private void hide(){
        singlePlayerMessage.SetActive(false);
    }

    private void show(){
        singlePlayerMessage.SetActive(true);
    }
}
