using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SetRoomCode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI code;
    private void Start()
    {
        if(TerraNovaManager.Instance != null){
            code.text = TerraNovaManager.Instance.getRelayCode();
        }
    }

    
}
