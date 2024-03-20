using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ResourceHandler : NetworkBehaviour
{

    [SerializeField] private string resourceName;
    [SerializeField] private double startHealth;
    [SerializeField] private NetworkVariable<double> currentHealth = new NetworkVariable<double>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Material defaultMaterial;
    private Material blinkMaterial;

    [SerializeField] private ResourceGenericHandler resourceDrop;

    [SerializeField] private int amountDropFrom = 1;
    [SerializeField] private int amountDropTo = 1;   

    [SerializeField] private ResourceEnum.ResourceType ToolBonus;


    [SerializeField] private NetworkVariable<int> randomAmountToDrop = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    

    private Renderer resourceRenderer; // Reference to the resource renderer

    public override void OnNetworkSpawn(){
        if(IsServer){currentHealth.Value = startHealth;}

        if(IsServer){
            randomAmountToDrop.Value = Random.Range(amountDropFrom, amountDropTo + 1);
        }
    }


    public void giveDamage(double damageAmount, ResourceEnum.ResourceType toolTypeUsed)
    {      
        if (currentHealth.Value >= 0)
        {
            if (toolTypeUsed == ToolBonus)
            {
                setNewHealthServerRpc(damageAmount);
            }
            else
            {
                setNewHealthServerRpc(damageAmount/2);
            }
        }
    }

    //run by the server, everyone can call
    [ServerRpc(RequireOwnership = false)]
    private void setNewHealthServerRpc(double value){
        currentHealth.Value -= value;

        checkHealthStats();
    }

   private void checkHealthStats()
    {
        if(!IsServer) return;

        if (currentHealth.Value <= 0)
        {
            // Resource died
            

            // Get the collider of the original GameObject
            Collider collider = GetComponent<Collider>();

            // Check if a collider is found
            if (collider != null)
            {
                // Disable colliders to prevent interactions
                collider.enabled = false;

                // Calculate the center of the GameObject
                Vector3 center = CalculateGameObjectCenter();

                // Check if the calculated center is valid
                if (!float.IsNaN(center.x) && !float.IsNaN(center.y) && !float.IsNaN(center.z))
                {
                    
                    dropItem(center, randomAmountToDrop.Value);
                    // Enable colliders after instantiation
                    collider.enabled = true;

                    // Destroy the original GameObject
                    destroyGameObject();
                    
                }
                else
                {
                    Debug.LogWarning("Invalid center position. Unable to instantiate dropped item.");
                    // Enable colliders since instantiation failed
                    collider.enabled = true;
                }
            }
        }
    }

    // RPC to destroy the object on all clients
    private void destroyGameObject()
    {   
        if(!IsServer) return;
        // Destroy the object on all clients
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }


    private void dropItem(Vector3 center, int randomAmount){
        // Check if this instance is the server
        if (!IsServer) return;

        // Instantiate the item at the center of the original GameObject
        GameObject droppedItem = Instantiate(resourceDrop.getDropGameObject(), center, Quaternion.identity);

        // Get the NetworkObject component of the instantiated item
        NetworkObject objectInNetwork = droppedItem.GetComponent<NetworkObject>();

        //add a droppedResource to the server copy and call the onnetworkspawn so it can get destroyed.
        DroppedResource droppedResourceComponentServer = addResourceComponent(objectInNetwork);

        droppedResourceComponentServer.OnNetworkSpawn();

        // Spawn the item across the network
        objectInNetwork.GetComponent<NetworkObject>().Spawn();

        

        // Call the client RPC to synchronize resource information
        setResourceInfoClientRpc(droppedItem.GetComponent<NetworkObject>().NetworkObjectId, randomAmount);
    }

    // Client RPC method to synchronize resource information
    [ClientRpc]
    private void setResourceInfoClientRpc(ulong objectId, int randomAmount){
        // Find the object with the specified network ID
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        // Check if the object exists
        if (networkObject){
            DroppedResource droppedResourceComponent = addResourceComponent(networkObject);

            
            // Set the resource information
            droppedResourceComponent.setResource(randomAmount, resourceDrop);

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

    private Vector3 CalculateGameObjectCenter()
    {
        // Get the renderer of the original GameObject
        Renderer renderer = GetComponent<Renderer>();

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

    

    public double getStartHealth(){
        return startHealth;
    }

    public double getCurrentHealth(){
        return currentHealth.Value;
    }

    public string getName(){
        return resourceName;
    }

    public int getRandomAmount(){
        return randomAmountToDrop.Value;
    }

    public ResourceGenericHandler getResourceDrop(){
        return resourceDrop;
    }
}