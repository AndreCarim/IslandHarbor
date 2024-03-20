using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Cleanup : MonoBehaviour
{
    private void Awake(){
        if(NetworkManager.Singleton != null){
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if(TerraNovaManager.Instance != null){
            Destroy(TerraNovaManager.Instance.gameObject);
        }

    }
    
}
