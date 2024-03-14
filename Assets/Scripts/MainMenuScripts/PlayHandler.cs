using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHandler : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button quitButton;

    private void Start(){
        multiplayerButton.onClick.AddListener(playMultiplayer);
        quitButton.onClick.AddListener(quit);
        singlePlayerButton.onClick.AddListener(playSingleplayer);
    }

    private void playMultiplayer(){
        TerraNovaManager.playMultiplayer = true;
        SceneManager.LoadScene("LobbyScene");
    }

    private void playSingleplayer(){
        TerraNovaManager.playMultiplayer = false;
        SceneManager.LoadScene("LobbyScene");
    }

    private void quit(){
        // Quit the application
        Application.Quit();
    }
}
