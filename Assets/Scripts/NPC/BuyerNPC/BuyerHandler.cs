using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BuyerHandler : NetworkBehaviour
{
    [SerializeField] private ResourceGenericHandler gold;
    [SerializeField] private GameObject dropPlace;

    private GameObject currentObject;


    void OnTriggerEnter(Collider collider)
    {
        if(!IsServer)return;
        
        GameObject resourceObject = collider.gameObject;

        if(!resourceObject)return;

        DroppedResource droppedResourceScript = resourceObject.GetComponent<DroppedResource>();

        if(!droppedResourceScript) return;

        currentObject = resourceObject;

        int resourceAmount = droppedResourceScript.getAmount();
        ResourceGenericHandler resource = droppedResourceScript.getResource();

        int goldAmount = resourceAmount * resource.getGoldValue();

        Vector3 center = CalculateGameObjectCenter(dropPlace);

        dropGoldServerRpc(center, goldAmount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void dropGoldServerRpc(Vector3 center, int amount){
        // Check if this instance is the server
        if (!IsServer) return;

        // Instantiate the item at the center of the original GameObject
        GameObject droppedItem = Instantiate(gold.getDropGameObject(), center, Quaternion.identity);

        // Get the NetworkObject component of the instantiated item
        NetworkObject objectInNetwork = droppedItem.GetComponent<NetworkObject>();

        //add a droppedResource to the server copy and call the onnetworkspawn so it can get destroyed.
        DroppedResource droppedResourceComponentServer = addResourceComponent(objectInNetwork);

        droppedResourceComponentServer.OnNetworkSpawn();

        // Spawn the item across the network
        objectInNetwork.GetComponent<NetworkObject>().Spawn();

        

        // Call the client RPC to synchronize resource information
        setResourceInfoClientRpc(droppedItem.GetComponent<NetworkObject>().NetworkObjectId, amount);

        destroyGameObject();
    }

    // Client RPC method to synchronize resource information
    [ClientRpc]
    private void setResourceInfoClientRpc(ulong objectId, int amount){
        // Find the object with the specified network ID
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        // Check if the object exists
        if (networkObject){
            DroppedResource droppedResourceComponent = addResourceComponent(networkObject);

            
            // Set the resource information
            droppedResourceComponent.setResource(amount, gold);

            // Notify the DroppedResource component that it has been spawned over the network
            droppedResourceComponent.OnNetworkSpawn();
        }
    }

    private DroppedResource addResourceComponent(NetworkObject networkObject){
         // Add or get the DroppedResource component of the object
        DroppedResource droppedResourceComponent = networkObject.GetComponent<DroppedResource>();
        if (droppedResourceComponent == null) {
            droppedResourceComponent = networkObject.gameObject.AddComponent<DroppedResource>();
        }

        return droppedResourceComponent;
    }

    // RPC to destroy the object on all clients
    private void destroyGameObject()
    {   
        if(!IsServer) return;
        // Destroy the object on all clients
        currentObject.GetComponent<NetworkObject>().Despawn(true);
    }

    private Vector3 CalculateGameObjectCenter(GameObject dropPosition)
    {
        // Get the renderer of the original GameObject
        Renderer renderer = dropPosition.GetComponent<Renderer>();

        // Check if a renderer is found
        if (renderer != null)
        {
            // Calculate the center of the renderer's bounds
            return renderer.bounds.center;
        }
        else
        {
            // If no renderer is found, return the position of the GameObject
            return transform.position;
        }
    }
}
