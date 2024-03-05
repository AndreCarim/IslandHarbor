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

    [SerializeField] private CameraInput cameraScript;
    [SerializeField] private InputMovement movementScript;
    [SerializeField] private RayCastingHandler clickScript;

    [SerializeField] private int currentMaxCarryWeight = 500;
    [SerializeField] private int currentCarryWeight = 0;
 
    private ResourceGenericHandler resourceSelected;
    private GameObject slot;
    private Camera mainCamera;

    private Coroutine shakeCoroutine;

    private bool canOpenInventory = true;
    private bool canCloseInventory = true;//if the menu is open, player cant close the inventory

    [SerializeField] private ResourceList resourceList;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    void Start(){
        mainCamera = Camera.main;

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        onFoot.OpenInventory.started += ctx => openInventory();

        onFoot.Enable();
    }


    private void openInventory(){
          // Check if the client is the owner of the object
        if (!IsOwner || !canOpenInventory || !canCloseInventory)
            return; // Exit the method if not the owner

       
        // Toggle the visibility of the inventory UI panel
        bool isOpen = inventoryUIHandlerScript.checkActiveInventory();
        inventoryUIHandlerScript.setWeight(currentCarryWeight + gameObject.GetComponent<ToolHandler>().getCurrentWeightEquipped(), currentMaxCarryWeight);

        // Call different functions based on the inventory state
        if (isOpen)
        {
            //oppening the inventory
            cameraScript.setIsFreeToLook(false); //lock the mouse
            movementScript.setCanWalk(false); // lock walking
            clickScript.setIsAnyUIOpen(true); // lock clicking 
            inventoryUIHandlerScript.setUIInfo(false); // closing the uiInfo
            inventoryUIHandlerScript.setStatsUI(false); // closing the uiStats
            slot = null;
        }
        else
        {
                
            //closing the inventory
            cameraScript.setIsFreeToLook(true); // unlock the mouse
            movementScript.setCanWalk(true); // unlock the walk
            clickScript.setIsAnyUIOpen(false); // unlock clicking
            inventoryUIHandlerScript.setToolTip(false);
        }
    }

    // Method to add a resource to the inventory, called by the raycastingHandler script
    public void AddResource(ResourceGenericHandler resource, int amount, GameObject collectibleResource = null)
    {
          // Check if the client is the owner of the object
        if (!IsOwner || !resource || amount == 0)
            return; // Exit the method if not the owner

        // Check if the resource ID already exists in the dictionary
        if (resourceCountDictionary.ContainsKey(resource.getId()))
        {
            // If it exists, increment its count
            int weightToAdd = resource.getWeight() * amount;

            // Check if the player has enough space to carry all the items
            int remainingWeightCapacity = currentMaxCarryWeight - (currentCarryWeight + gameObject.GetComponent<ToolHandler>().getCurrentWeightEquipped());
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
                if(collectibleResource){
                     //it means that the addResoure is being called by the raycastHandler, the player is trying to get an item
                    //from the floor and it has no space in the inventory, so it will drop in front of him
                    //so i need to send the networkobjectid to the dropiteminfrontoftheplayer so it can know which kind of object it needs to destroy
                    dropItemInFrontOfThePlayer(amount - maxAmount, resource, true, CalculateXPositionToDrop(), CalculateYPositionToDrop(), CalculateZPositionToDrop(), collectibleResource.GetComponent<NetworkObject>().NetworkObjectId);
                }else{
                    dropItemInFrontOfThePlayer(amount - maxAmount, resource, true, CalculateXPositionToDrop(), CalculateYPositionToDrop(), CalculateZPositionToDrop());
                }
            }
                
            
        }else
        {
            // If it does not exist, add its count
            int weightToAdd = resource.getWeight() * amount;

            // Check if the player has enough space to carry all the items
            int remainingWeightCapacity =  currentMaxCarryWeight - (currentCarryWeight + gameObject.GetComponent<ToolHandler>().getCurrentWeightEquipped());
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
                if(collectibleResource){
                     //it means that the addResoure is being called by the raycastHandler, the player is trying to get an item
                    //from the floor and it has no space in the inventory, so it will drop in front of him
                    //so i need to send the networkobjectid to the dropiteminfrontoftheplayer so it can know which kind of object it needs to destroy
                    dropItemInFrontOfThePlayer(amount - maxAmount, resource, true, CalculateXPositionToDrop(), CalculateYPositionToDrop(), CalculateZPositionToDrop(), collectibleResource.GetComponent<NetworkObject>().NetworkObjectId);
                }else{
                    dropItemInFrontOfThePlayer(amount - maxAmount, resource, true, CalculateXPositionToDrop(), CalculateYPositionToDrop(), CalculateZPositionToDrop());
                }
               
            }
        }

        // Create UI for the new resource
        inventoryUIHandlerScript.CreateOrUpdateResourceUI(resource, resourceCountDictionary[resource.getId()]);

        if(collectibleResource){
             //destroy via server so all the clients will get
             //only will do if the player is trying to get the resource from the floor
             //if its from the inventory or when the player earns from npc, it dosent need to call destroy
             //since there is no object to destroy in the game
             destroyObjectServerRpc(collectibleResource.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void destroyObjectServerRpc(ulong objectId){
        // Find the object with the specified network ID
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        if(networkObject){
            networkObject.Despawn();
        }
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

                //now checking if i am trying to remove a resource that is in the inventory or in the equipment equipped
                if(!slot || !slot.CompareTag("EquipmentIcon")){//if there is no slot, the player is removing items from an npc

                    // Remove UI for the resource
                    inventoryUIHandlerScript.RemoveResourceUI(resource.getId());     
                }else if(slot && slot.CompareTag("EquipmentIcon")){

                    //trying to drop a item from the equipedEquipment
                    gameObject.GetComponent<ToolHandler>().removeEquippedEquipmentByDropOrUnequip(resource);
                }
                inventoryUIHandlerScript.setUIInfo(false);
                inventoryUIHandlerScript.setStatsUI(false);       
            }
            else
            {

                if(slot && slot.CompareTag("EquipmentIcon")){
                    //if the player dropped the item from the equipped area 
                    //but the player still have more, it will remove the equipped item
                    gameObject.GetComponent<ToolHandler>().removeEquippedEquipmentByDropOrUnequip(resource);    

                }

                // Update UI for the resource
                inventoryUIHandlerScript.CreateOrUpdateResourceUI(resource, resourceCountDictionary[resource.getId()]);
 
                if(slot){
                    inventoryUIHandlerScript.StartShaking();
                }
                
            }

            currentCarryWeight -= resource.getWeight() * amount;
        }
    }

    public void slotSelected(ResourceGenericHandler resourceSelected, GameObject slot, bool isEquipped = false){
    // Check if the selected slot is different from the current one
        if (this.slot != slot)
        {
            this.resourceSelected = resourceSelected;
            this.slot = slot;

            if(resourceCountDictionary.ContainsKey(resourceSelected.getId()) && !slot.CompareTag("EquipmentIcon")){
                inventoryUIHandlerScript.setResourceSelected(resourceSelected, slot, resourceCountDictionary[resourceSelected.getId()], isEquipped);
            }else{
                //if there is no resource in the inventory, it means its the equipped and there is no resource in the invetory
                //so the player has only one
 
                inventoryUIHandlerScript.setResourceSelected(resourceSelected, slot, 1, isEquipped);
            }

       }
    }

    //this is being called by the button component on the dropButton isnde the inventory UI
    public void dropButtonPressed()
    {
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner

        // Check if the resource is in the inventory
        if (!slot.CompareTag("EquipmentIcon") && resourceCountDictionary.ContainsKey(resourceSelected.getId()))
        {//making sure we have in the inventory and its not an equipment
            
            // Get the amount of the selected resource
            int amountPlayerHas = resourceCountDictionary.ContainsKey(resourceSelected.getId()) ? resourceCountDictionary[resourceSelected.getId()] : 0;
            // Check if the player has enough resource
            int amountToDrop = inventoryUIHandlerScript.getAmountToDropSlider();
            if (amountPlayerHas >= amountToDrop)
            {

                inventoryUIHandlerScript.stopCoroutine();

                RemoveResource(resourceSelected, amountToDrop); //from the inventory ui  
                dropItemInFrontOfThePlayer(amountToDrop, resourceSelected, false, CalculateXPositionToDrop(), CalculateYPositionToDrop(), CalculateZPositionToDrop());   
                
                inventoryUIHandlerScript.setWeight(currentCarryWeight + gameObject.GetComponent<ToolHandler>().getCurrentWeightEquipped(), currentMaxCarryWeight);
            }
        }
        else if(slot.CompareTag("EquipmentIcon"))
        {
            //player trying to drop a equipment from the equipped tab and it only has one
            //remove the equipment from the toolhandler and then
            //drop in front of the player
            gameObject.GetComponent<ToolHandler>().removeEquippedEquipmentByDropOrUnequip(resourceSelected);
            dropItemInFrontOfThePlayer(1, resourceSelected, false, CalculateXPositionToDrop(), CalculateYPositionToDrop(), CalculateZPositionToDrop());
            
            inventoryUIHandlerScript.setUIInfo(false);
            inventoryUIHandlerScript.setStatsUI(false);
            inventoryUIHandlerScript.setWeight(currentCarryWeight + gameObject.GetComponent<ToolHandler>().getCurrentWeightEquipped(), currentMaxCarryWeight);
            inventoryUIHandlerScript.stopCoroutine();
        }else{
            // If the resource is not in the inventory and not in the equipments equipped, do nothing. THIS SHOULD NOT HAPPEN!
            Debug.Log("How are you clicking on a item that you dont have? \n and why are you seeing this message???");
        }
    }

    private void dropItemInFrontOfThePlayer(int amountToDrop, ResourceGenericHandler resourceToDrop, bool isExtra, float x, float y, float z, ulong objectToCollectId = 0){   
        
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner
        

        if(!isExtra){//meaning that it is dropping from the inventory, if its true, its dropping from inventory
            //resource dropped from inventory
            instantiateItemFromDroppingInServerRpc(amountToDrop, resourceToDrop.getId(), x, y, z);
            
            if(resourceCountDictionary.ContainsKey(resourceSelected.getId())){
                inventoryUIHandlerScript.setItemInformation(resourceToDrop, resourceCountDictionary.ContainsKey(resourceSelected.getId()), resourceCountDictionary[resourceSelected.getId()]);
            }  
        }else{
            //resource dropped from max weight
            //its dropping because player cant take more weight
            if(objectToCollectId != 0){
                instantiateItemFromCollectingInServerRpc(amountToDrop, objectToCollectId, x, y, z);
            }
        }
    }

    //TO DROP FROM MAX WEIGHT
    [ServerRpc(RequireOwnership = false)]
    private void instantiateItemFromCollectingInServerRpc(int amountToDrop, ulong objectToCollectId, float x, float y, float z){

         // Check if this instance is the server
        if (!IsServer) return;

         // Find the object with the specified network ID
        NetworkObject objectToCollectNetwork = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectToCollectId];

        // Instantiate the item at the center of the original GameObject
        GameObject droppedItem = Instantiate(objectToCollectNetwork.GetComponent<DroppedResource>().getResource().getDropGameObject(), new Vector3(x, y, z), Quaternion.identity);

        // Get the NetworkObject component of the instantiated item
        NetworkObject objectInNetwork = droppedItem.GetComponent<NetworkObject>();

         //add a droppedResource to the server copy and call the onnetworkspawn so it can get destroyed.
        DroppedResource droppedResourceComponentServer = addResourceComponent(objectInNetwork);

        droppedResourceComponentServer.OnNetworkSpawn();

        // Spawn the item across the network
        objectInNetwork.GetComponent<NetworkObject>().Spawn();
         // Call the client RPC to synchronize resource information
        setResourceInfoFromCollectingClientRpc(objectInNetwork.NetworkObjectId, amountToDrop, objectToCollectId);
    }

    [ClientRpc]
    private void setResourceInfoFromCollectingClientRpc(ulong objectId, int randomAmount, ulong objectToCollectId){
        // Find the object with the specified network ID
        NetworkObject objectToCollectNetwork = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectToCollectId];

        // Find the object with the specified network ID
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        // Check if the object exists
        if (networkObject && objectToCollectNetwork){
            // Add or get the DroppedResource component of the object
            DroppedResource droppedResourceComponent = addResourceComponent(networkObject);           

            // Set the resource information
            droppedResourceComponent.setResource(randomAmount, objectToCollectNetwork.GetComponent<DroppedResource>().getResource());

            // Notify the DroppedResource component that it has been spawned over the network
            droppedResourceComponent.OnNetworkSpawn();
        }
    }
    //TO DROP FROM MAX WEIGHT



    //TO DROP FROM INVENTORY
    [ServerRpc(RequireOwnership = false)]
    private void instantiateItemFromDroppingInServerRpc(int amount, int resourceIndex, float x, float y, float z){
         // Check if this instance is the server
        if (!IsServer) return;

        //Getting the resourceDropped
        ResourceGenericHandler resourceDropped = resourceList.resourceList[resourceIndex];

        if(resourceDropped){
             // Instantiate the item at the center of the original GameObject
            GameObject droppedItem = Instantiate(resourceDropped.getDropGameObject(), new Vector3(x, y, z), Quaternion.identity);
            
            // Get the NetworkObject component of the instantiated item
            NetworkObject objectInNetwork = droppedItem.GetComponent<NetworkObject>();

             //add a droppedResource to the server copy and call the onnetworkspawn so it can get destroyed.
            DroppedResource droppedResourceComponentServer = addResourceComponent(objectInNetwork);

            droppedResourceComponentServer.OnNetworkSpawn();

            // Spawn the item across the network
            objectInNetwork.GetComponent<NetworkObject>().Spawn();
            // Call the client RPC to synchronize resource information
            setResourceInfoFromDroppingClientRpc(objectInNetwork.NetworkObjectId, amount, resourceIndex);
        }

    }

    [ClientRpc]
    private void setResourceInfoFromDroppingClientRpc(ulong objectId, int amount, int resourceIndex){
        // Find the object with the specified network ID
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        // Check if the object exists
        if (networkObject){
            // Add or get the DroppedResource component of the object
            DroppedResource droppedResourceComponent = addResourceComponent(networkObject);
            
            // Set the resource information
            droppedResourceComponent.setResource(amount, resourceList.resourceList[resourceIndex]);

            // Notify the DroppedResource component that it has been spawned over the network
            droppedResourceComponent.OnNetworkSpawn();
        }
    }
    //TO DROP FROM INVENTORY
    
    private DroppedResource addResourceComponent(NetworkObject networkObject){
        DroppedResource droppedResourceComponent = networkObject.GetComponent<DroppedResource>();
        if (droppedResourceComponent == null) {
            droppedResourceComponent = networkObject.gameObject.AddComponent<DroppedResource>();
        }

        return droppedResourceComponent;
    }

    /*
    this will be called when the client presses the equipButton on the inventory
    Then this will check which kind of equipment the client is trying to equip
    then it will call the ToolHandler so the equip can happen
    then it will take the last tool put it back to the inventory and then 
    /destroy the last equipment button inside the inventory
    */
    public void equipButtonPressed(){
        if(!IsOwner) return;

        if(resourceSelected.getResourceType() == ResourceEnum.ResourceType.Axe){
            //equipping an axe
            equipButtonPressedHandler(gameObject.GetComponent<ToolHandler>().getCurrentAxe());
        }else if(resourceSelected.getResourceType() == ResourceEnum.ResourceType.PickAxe){
            //equipping a pickAxe
            equipButtonPressedHandler(gameObject.GetComponent<ToolHandler>().getCurrentPickAxe()); 
        }else if(resourceSelected.getResourceType() == ResourceEnum.ResourceType.Weapon){
            equipButtonPressedHandler(gameObject.GetComponent<ToolHandler>().getCurrentWeapon());        
        }
    }

    private void equipButtonPressedHandler(ResourceGenericHandler currentResource){
        //if its trying to equip the same equipment, return;
        if(currentResource == resourceSelected || !IsOwner){ return;}

        

        gameObject.GetComponent<ToolHandler>().equipEquippment(resourceSelected);    

        
        RemoveResource(resourceSelected, 1);//remove the current one
        AddResource(currentResource, 1);

        if(currentResource){
            // Update UI for the resource, adding the equipment to the inventory
            //only happening if the player had a equipment already equipped. 
            inventoryUIHandlerScript.CreateOrUpdateResourceUI(currentResource, resourceCountDictionary[currentResource.getId()]);
        }

        setSlotNull();

        inventoryUIHandlerScript.setUIInfo(false);
        inventoryUIHandlerScript.setStatsUI(false);
        inventoryUIHandlerScript.stopCoroutine();
    }

    public void unequipButtonPressed(){
        //addResource
        //removeEquippedEquipmentByDropOrUnequip
        if(resourceSelected){
             AddResource(resourceSelected, 1);

             //trying to drop a item from the equipedEquipment
            gameObject.GetComponent<ToolHandler>().removeEquippedEquipmentByDropOrUnequip(resourceSelected);
        }

       
        setSlotNull();

        inventoryUIHandlerScript.setUIInfo(false);
        inventoryUIHandlerScript.setStatsUI(false);
        inventoryUIHandlerScript.stopCoroutine();
    }

    private void setSlotNull(){
        slot = null;
        resourceSelected = null;

        inventoryUIHandlerScript.setSlotNull();
    }

    private float CalculateXPositionToDrop()
    {
        // Ensure we have a main camera
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return 0f;
        }

        // Calculate the x position in front of the camera
        float distanceFromCamera = 1.5f; // Adjust this value to control the distance from the camera
        float xPositionInFrontOfCamera = mainCamera.transform.position.x + mainCamera.transform.forward.x * distanceFromCamera;

        return xPositionInFrontOfCamera;
    }

    private float CalculateYPositionToDrop()
    {
        // Ensure we have a main camera
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return 0f;
        }

        // Calculate the y position in front of the camera
        // For simplicity, let's just return the y position of the camera
        return mainCamera.transform.position.y;
    }

    private float CalculateZPositionToDrop()
    {
        // Ensure we have a main camera
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return 0f;
        }

        // Calculate the z position in front of the camera
        float distanceFromCamera = 1.5f; // Adjust this value to control the distance from the camera
        float zPositionInFrontOfCamera = mainCamera.transform.position.z + mainCamera.transform.forward.z * distanceFromCamera;

        return zPositionInFrontOfCamera;
    }

    public void setOnStart(GameObject entirePlayerUIInstance){
          // Check if the client is the owner of the object
        if (!IsOwner)
            return; // Exit the method if not the owner
        
        inventoryUIHandlerScript = entirePlayerUIInstance.GetComponent<InventoryUIHandler>();

        inventoryUIHandlerScript.setWeight(currentCarryWeight + gameObject.GetComponent<ToolHandler>().getCurrentWeightEquipped(), currentMaxCarryWeight);
        inventoryUIHandlerScript.setPlayer(gameObject);
    }

    public int checkItemAmount(int id){
        if(resourceCountDictionary.ContainsKey(id)){
            return resourceCountDictionary[id];
        }else{
            return 0;
        }
    }

    public void setCanOpenInventory(bool value){
        canOpenInventory = value;
    }
   
    public void setCanCloseInventory(bool value){
        canCloseInventory = value;
    }
}