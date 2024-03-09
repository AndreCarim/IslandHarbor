using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class InGameMenuhandler : MonoBehaviour
{
    [SerializeField] private Transform menuWindow;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button openInGameMenuButton;
    [SerializeField] private Button quitGame;

    [SerializeField] private ResourceInventory inventoryScript;

    private bool isMenuOpen = false;

    private PlayerInput playerInput;
    private PlayerInput.GeneralInputActions generalInput;

    private void Awake(){
        returnButton.onClick.AddListener(hide);
        openInGameMenuButton.onClick.AddListener(show);
        quitGame.onClick.AddListener(quit);

        mainMenuButton.onClick.AddListener(sendPlayerToMainMenu);

        playerInput = new PlayerInput();
        generalInput = playerInput.GeneralInput;

        generalInput.OpenMenu.started += ctx => show();

        generalInput.Enable();

    }


    private void sendPlayerToMainMenu(){
        ClientsHandler.Instance.serverIsShuttingDown();

        NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene("MainMenu");
    }

    private void hide(){
        isMenuOpen = false;
        menuWindow.gameObject.SetActive(false);

        inventoryScript.setCanCloseInventory(true);
    }

    private void show(){
        isMenuOpen = true;
        menuWindow.gameObject.SetActive(true);
        

        inventoryScript.setCanCloseInventory(false);
    }

    public bool getIsMenuOpen(){
        return isMenuOpen;
    }

     private void quit(){
        // Quit the application
        Application.Quit();
    }

    private void OnDisable() {
        generalInput.Disable();
    }
}
