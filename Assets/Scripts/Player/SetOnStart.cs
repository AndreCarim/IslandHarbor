using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//this script will handle all the assigning for the variables inside the player scripts
public class SetOnStart : NetworkBehaviour
{

    private ToolHandler toolHandlerScript;
    private ResourceInventory resourceInventoryScript;
    private RayCastingHandler rayCastingHandlerScript;

    [SerializeField] private GameObject entirePlayerUI;

    private void initialize()
    {
        // Check if this ui belongs to the local player
        if (IsOwner)
        {
            entirePlayerUI.SetActive(true);
        }
        else
        {
            entirePlayerUI.SetActive(false);
        }
    }

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        initialize();

        if(!IsOwner){return;}
        
        toolHandlerScript = gameObject.GetComponent<ToolHandler>();
        resourceInventoryScript = gameObject.GetComponent<ResourceInventory>();

        // Get the RayCastingHandler component from children
        rayCastingHandlerScript = gameObject.GetComponentInChildren<RayCastingHandler>();

        setOnStart();
    }


    private void setOnStart(){

        if(!IsOwner){return;}

        toolHandlerScript.setOnStart(entirePlayerUI);
        resourceInventoryScript.setOnStart(entirePlayerUI);
        rayCastingHandlerScript.setOnStart(entirePlayerUI);
    }

   
}
