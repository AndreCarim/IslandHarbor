using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class ResourceInventory : NetworkBehaviour
{
    // This dictionary will store the item ID as the key and the quantity as the value.
    [SerializeField] private Dictionary<int, int> resourceCountDictionary = new Dictionary<int, int>();

    //  This dictionary will store the item ID as the key and the corresponding UI GameObject as the value.
    public Dictionary<int, int> ResourceCountDictionary => resourceCountDictionary;

    private InventoryUIHandler inventoryUIHandlerScript;

    [SerializeField] private FirstPersonCameraHandler cameraScript;
    [SerializeField] private Movement movementScript;
    [SerializeField] private RayCastingHandler clickScript;

    [SerializeField] private int currentMaxCarryWeight = 500;
    [SerializeField] private int currentCarryWeight = 0;
 
    private ResourceGenericHandler resourceSelected;
    private GameObject slot;
    private Camera mainCamera;

    private Coroutine shakeCoroutine;
   

    void Start(){
        mainCamera = Camera.main;
    }

    void Update()
    {

          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner

        // Check if the player presses the Tab or I key
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            // Toggle the visibility of the inventory UI panel
            bool isOpen = inventoryUIHandlerScript.checkActiveInventory();
            inventoryUIHandlerScript.setWeight(currentCarryWeight, currentMaxCarryWeight);

             // Call different functions based on the inventory state
            if (isOpen)
            {
                cameraScript.setIsFreeToLook(false); //lock the mouse
                movementScript.setCanWalk(false); // lock walking
                clickScript.setInventoryIsOpen(true); // lock clicking
                inventoryUIHandlerScript.setUIInfo(false);
                slot = null;
            }
            else
            {
                cameraScript.setIsFreeToLook(true); // unlock the mouse
                movementScript.setCanWalk(true); // unlock the walk
                clickScript.setInventoryIsOpen(false); // unlock clicking
                inventoryUIHandlerScript.setToolTip(false);
            }
        }
    }

    // Method to add a resource to the inventory
    public void AddResource(ResourceGenericHandler resource, int amount, GameObject collectibleResource)
    {
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner

        // Check if the resource ID already exists in the dictionary
        if (resourceCountDictionary.ContainsKey(resource.getId()))
        {
            // If it exists, increment its count
            int weightToAdd = resource.getWeight() * amount;

            // Check if the player has enough space to carry all the items
            int remainingWeightCapacity = currentMaxCarryWeight - currentCarryWeight;
            if (weightToAdd <= remainingWeightCapacity)
            {
                // The player has space to carry all the items
                resourceCountDictionary[resource.getId()] += amount;
                currentCarryWeight += weightToAdd;
            }
            else//resourceInforUI
            {
                // The player doesn't have enough space for all items
                // Calculate the maximum number of items the player can pick up without exceeding the weight limit
                int maxAmount = remainingWeightCapacity / resource.getWeight();
                // Add only the maximum possible amount
                resourceCountDictionary[resource.getId()] += maxAmount;
                currentCarryWeight += maxAmount * resource.getWeight();

                //drop the remaining
                dropItemInFrontOfThePlayer(amount - maxAmount, resource, true);
            }
            
        }else
        {
            // If it does not exist, add its count
            int weightToAdd = resource.getWeight() * amount;

            // Check if the player has enough space to carry all the items
            int remainingWeightCapacity = currentMaxCarryWeight - currentCarryWeight;
            if (weightToAdd <= remainingWeightCapacity)
            {
                // The player has space to carry all the items
                resourceCountDictionary.Add(resource.getId(), amount);
                currentCarryWeight += weightToAdd;
            }
            else
            {

                // The player doesn't have enough space for all items
                // Calculate the maximum number of items the player can pick up without exceeding the weight limit
                int maxAmount = remainingWeightCapacity / resource.getWeight();

                // Add only the maximum possible amount
                resourceCountDictionary.Add(resource.getId(), maxAmount);

                currentCarryWeight += maxAmount * resource.getWeight();
                //drop the remaining
               dropItemInFrontOfThePlayer(amount - maxAmount, resource, true);
            }
        }

        // Create UI for the new resource
        inventoryUIHandlerScript.CreateOrUpdateResourceUI(resource, resourceCountDictionary[resource.getId()]);
        Destroy(collectibleResource);
    }

    // Method to remove a resource from the inventory
    public void RemoveResource(ResourceGenericHandler resource, int amount)
    {
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner

        // Check if the resource ID exists in the dictionary
        if (resourceCountDictionary.ContainsKey(resource.getId()))
        {
            // Decrement the resource count
            resourceCountDictionary[resource.getId()] -= amount;

            // If count goes <= 0, remove it from the dictionary and UI
            if (resourceCountDictionary[resource.getId()] <= 0)
            {
                resourceCountDictionary.Remove(resource.getId());

                // Remove UI for the resource
                inventoryUIHandlerScript.RemoveResourceUI(resource.getId());

                inventoryUIHandlerScript.setUIInfo(false);
            }
            else
            {
                // Update UI for the resource
                inventoryUIHandlerScript.CreateOrUpdateResourceUI(resource, resourceCountDictionary[resource.getId()]);

                inventoryUIHandlerScript.StartShaking();
            }
        }
    }

    public void slotSelected(ResourceGenericHandler resourceSelected, GameObject slot){
    // Check if the selected slot is different from the current one
        if (this.slot != slot)
        {
            this.resourceSelected = resourceSelected;
            this.slot = slot;

            inventoryUIHandlerScript.setResourceSelected(resourceSelected, slot, resourceCountDictionary[resourceSelected.getId()]);
        }
    }

    //this is being called by the button component on the dropButton isnde the inventory UI
    public void dropButtonPressed()
    {
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner

        // Check if the resource is in the inventory
        if (resourceCountDictionary.ContainsKey(resourceSelected.getId()))
        {
            
            // Get the amount of the selected resource
            int amountPlayerHas = resourceCountDictionary.ContainsKey(resourceSelected.getId()) ? resourceCountDictionary[resourceSelected.getId()] : 0;
            // Check if the player has enough resource
            int amountToDrop = inventoryUIHandlerScript.getAmountToDropSlider();
            if (amountPlayerHas >= amountToDrop)
            {//inventoryUIHandler

                inventoryUIHandlerScript.stopCoroutine();

                currentCarryWeight -= resourceSelected.getWeight() * amountToDrop;

                RemoveResource(resourceSelected, amountToDrop); //from the inventory ui  
                dropItemInFrontOfThePlayer(amountToDrop, resourceSelected, false);   
                
                inventoryUIHandlerScript.setWeight(currentCarryWeight, currentMaxCarryWeight);
            }
        }
        else
        {
            // If the resource is not in the inventory, do nothing. THIS SHOULD NOT HAPPEN!
            Debug.Log("How are you clicking on a item that you dont have? \n and why are you seeing this message???");
        }
    }

    private void dropItemInFrontOfThePlayer(int amountToDrop, ResourceGenericHandler resourceToDrop, bool isExtra){   
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner
        
        // Calculate the center of the GameObject
        Vector3 positionToDrop = calculatePositionToDrop();

        // Instantiate the item at the center of the original GameObject
        GameObject droppedItem = Instantiate(resourceToDrop.getDropGameObject(), positionToDrop, Quaternion.identity);
        droppedItem.AddComponent<DroppedResource>();

        droppedItem.GetComponent<DroppedResource>().setResource(amountToDrop, resourceToDrop);

        // Change the layer to "ResourceFloating"
        droppedItem.layer = LayerMask.NameToLayer("CollectibleResource");

        if(!isExtra){//meaning that it is dropping from the inventory, if its true, its dropping
            if(resourceCountDictionary.ContainsKey(resourceSelected.getId())){
                inventoryUIHandlerScript.setItemInformation(resourceToDrop, resourceCountDictionary.ContainsKey(resourceSelected.getId()), resourceCountDictionary[resourceSelected.getId()]);
            }  
        }
    }

    private Vector3 calculatePositionToDrop()
    {
        // Ensure we have a main camera
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return Vector3.zero;
        }

        // Calculate the position in front of the camera
        float distanceFromCamera = 1.5f; // Adjust this value to control the distance from the camera
        Vector3 positionInFrontOfCamera = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

        return positionInFrontOfCamera;
    }

    public void setOnStart(GameObject entirePlayerUIInstance){
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner
        
        inventoryUIHandlerScript = entirePlayerUIInstance.GetComponent<InventoryUIHandler>();

        inventoryUIHandlerScript.setWeight(currentCarryWeight, currentMaxCarryWeight);
        inventoryUIHandlerScript.setPlayer(gameObject);
    }
}