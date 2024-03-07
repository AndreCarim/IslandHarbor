using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHandler : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;


    private void Start(){
        playButton.onClick.AddListener(play);
        quitButton.onClick.AddListener(quit);
    }

    private void play(){
        SceneManager.LoadScene("LobbyScene");
    }

    private void quit(){
        // Quit the application
        Application.Quit();
    }
}
