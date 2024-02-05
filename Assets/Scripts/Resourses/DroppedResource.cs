using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DroppedResource : NetworkBehaviour
{
    private int amount;
    private ResourceGenericHandler resource;

    public override void OnNetworkSpawn()
    {   
        if(IsServer) {
            // Generate a random time interval for destruction
            float destroyTime = 10f;

            // Destroy the GameObject after the random time interval
            Destroy(gameObject, destroyTime);
        }
        

         // Change the layer to "ResourceFloating"
        gameObject.layer = LayerMask.NameToLayer("CollectibleResource");
        
    }

    public void setResource(int amountFromServer, ResourceGenericHandler resource)
    {
        amount = amountFromServer;
        this.resource = resource;
    }

    public ResourceGenericHandler getResource(){
        return resource;
    }

    public int getAmount(){
        return amount;
    }
}
