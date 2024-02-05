using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ResourceHandler : NetworkBehaviour
{
    [SerializeField] private double startHealth;
    [SerializeField] private NetworkVariable<double> currentHealth = new NetworkVariable<double>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Material defaultMaterial;
    private Material blinkMaterial;

    [SerializeField] private ResourceGenericHandler resourceDrop;

    [SerializeField] private int amountDropFrom = 1;
    [SerializeField] private int amountDropTo = 1;   

    [SerializeField] private ToolEnum.ToolType toolType;


    

    private Renderer resourceRenderer; // Reference to the resource renderer

    public override void OnNetworkSpawn(){
        if(IsServer){currentHealth.Value = startHealth;}

         // Attempt to get the Renderer component from the same GameObject
        resourceRenderer = GetComponent<Renderer>();

        if (resourceRenderer == null)
        {
            Debug.LogError("ResourceHandler: Resource Renderer not found on the same GameObject!");
        }
        else
        {
            defaultMaterial = resourceRenderer.material;
        }
    }


    public void giveDamage(double damageAmount, ToolEnum.ToolType toolTypeUsed)
    {      
        if (currentHealth.Value >= 0)
        {
            if (toolTypeUsed == toolType)
            {
                setNewHealthServerRpc(damageAmount);
            }
            else
            {
                setNewHealthServerRpc(damageAmount/2);
            }
            StartCoroutine(Effects());
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
            // Get a random number between amountDropFrom (inclusive) and amountDropTo (exclusive)
            int randomAmount = Random.Range(amountDropFrom, amountDropTo + 1);

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
                    
                    dropItem(center, randomAmount);
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
            // Add or get the DroppedResource component of the object
            DroppedResource droppedResourceComponent = networkObject.GetComponent<DroppedResource>();
            if (droppedResourceComponent == null) {
                droppedResourceComponent = networkObject.gameObject.AddComponent<DroppedResource>();
            }

            // Set the resource information
            droppedResourceComponent.setResource(randomAmount, resourceDrop);

            // Notify the DroppedResource component that it has been spawned over the network
            droppedResourceComponent.OnNetworkSpawn();
        }
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

    IEnumerator Effects()
    {
        if (resourceRenderer != null)
        {
            // Store the original color and position
            Color originalColor = resourceRenderer.material.color;
            Vector3 originalPosition = transform.position;

            // Apply blink material with semi-transparent white color
            resourceRenderer.material = blinkMaterial;
            resourceRenderer.material.color = new Color(1f, .5f, .4f, 0f); // Adjust the alpha value for transparency

            // Shake parameters
            float shakeDuration = 0.15f; // Adjust the duration of the shaking
            float shakeMagnitude = 0.5f; // Adjust the magnitude of the shaking

            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                // Calculate the amount of shaking
                float x = originalPosition.x + Random.Range(-1f, 1f) * shakeMagnitude;
                float y = originalPosition.y + Random.Range(-1f, 1f) * shakeMagnitude;

                // Apply the shaking to the object's position
                transform.position = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Revert to the default material, original color, and position
            resourceRenderer.material = defaultMaterial;
            resourceRenderer.material.color = originalColor;
            transform.position = originalPosition;
        }
        else
        {
            Debug.LogError("ResourceHandler: Resource Renderer not found on the same GameObject!");
        }
    }

    public double getStartHealth(){
        return startHealth;
    }

    public double getCurrentHealth(){
        return currentHealth.Value;
    }

    public string getName(){
        return resourceDrop.getResourceName();
    }

    

}