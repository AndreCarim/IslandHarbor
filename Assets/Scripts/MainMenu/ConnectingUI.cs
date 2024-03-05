using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start(){
        MainMenuHandler.Instance.OnTryingToJoinGame += MainMenu_OnTryingToJoinGame;
        MainMenuHandler.Instance.OnFailedToJoinGame += MainMenu_OnFailedToJoinGame;

        hide();
    }

    private void MainMenu_OnFailedToJoinGame(object sender, System.EventArgs e){
        hide();
    }

    private void MainMenu_OnTryingToJoinGame(object sender, System.EventArgs e){
        Show();
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void hide(){
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MainMenuHandler.Instance.OnTryingToJoinGame -= MainMenu_OnTryingToJoinGame;
        MainMenuHandler.Instance.OnFailedToJoinGame -= MainMenu_OnFailedToJoinGame;
    }
}
