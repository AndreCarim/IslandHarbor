using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class LobbyButtonshandler : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;    

    private void Awake() {
        hostButton.onClick.AddListener(() => {
            TerraNovaManager.Instance.startHost();
        });

        clientButton.onClick.AddListener(() => {
            TerraNovaManager.Instance.startClient();
        });
    }
 
}
