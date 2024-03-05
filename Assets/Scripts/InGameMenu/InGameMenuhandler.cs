using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InGameMenuhandler : MonoBehaviour
{
    [SerializeField] private Transform menuWindow;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button openInGameMenuButton;

    [SerializeField] private ResourceInventory inventoryScript;

    private bool isMenuOpen = false;

    private PlayerInput playerInput;
    private PlayerInput.GeneralInputActions generalInput;

    private void Start(){
        returnButton.onClick.AddListener(hide);
        openInGameMenuButton.onClick.AddListener(show);

        mainMenuButton.onClick.AddListener(sendPlayerToMainMenu);

        playerInput = new PlayerInput();
        generalInput = playerInput.GeneralInput;

        generalInput.OpenMenu.started += ctx => show();

        generalInput.Enable();

    }


    private void sendPlayerToMainMenu(){
        Debug.Log("Go back to main menu");
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
}
