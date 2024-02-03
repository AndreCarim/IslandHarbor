using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    [SerializeField] private Camera testCamera;//

    private void Awake(){
        startHostButton.onClick.AddListener(() => {
            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
            hide();
        });

        startClientButton.onClick.AddListener(() => {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
            hide();
        });
    }

    private void hide(){
        gameObject.SetActive(false);
    }
    
}
