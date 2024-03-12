using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    [SerializeField] private Transform connectinUI;
    private void Start(){
        TerraNovaManager.Instance.OnTryingToJoinGame += MainMenu_OnTryingToJoinGame;
        TerraNovaManager.Instance.OnFailedToJoinGame += MainMenu_OnFailedToJoinGame;

        hide();
    }

    private void MainMenu_OnFailedToJoinGame(object sender, System.EventArgs e){
        hide();
    }

    private void MainMenu_OnTryingToJoinGame(object sender, System.EventArgs e){
        Show();
    }

    private void Show(){
        connectinUI.gameObject.SetActive(true);
    }

    private void hide(){
        connectinUI.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        TerraNovaManager.Instance.OnTryingToJoinGame -= MainMenu_OnTryingToJoinGame;
        TerraNovaManager.Instance.OnFailedToJoinGame -= MainMenu_OnFailedToJoinGame;
    }
}
