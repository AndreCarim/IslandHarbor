using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class RayCastingHandler : NetworkBehaviour
{
    private CenterUIHandler centerUIHandlerScript;
    private ResourceHPBar resourceHPBarScript;
    private NPCUIHandler npcUiHandler;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    [SerializeField] private float maxDistanceToInteract = 5.5f;
    [SerializeField] private ToolHandler toolHandler;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask playerLayer; //used to ignore the player
    

    [SerializeField] private Color isBreakableCenterColor = new Color(0.9529f, 0.6f, 0.6f); // Assigning a light red color
    [SerializeField] private Color CollectibleResourceCenterColor = new Color(0.6039f, 0.9804f, 0.6745f); // Assigning a color close to #9FFAAC
    [SerializeField] private Color interactableColor = new Color(1f, .9f, .1f);

    private GameObject activeTooltip;


    private bool canClick = true;
    private bool isAnyUIOpen = false;

    //toolTip for the collectible
    [SerializeField] private Transform tooltip;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI totalWeightText;
    [SerializeField] private Transform pressEToInteract;
    private GameObject currentLookingObject;
    //toolTip for the collectible
    


    private Ray ray;


    public override void OnNetworkSpawn(){
        if(!IsOwner) return;
        
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        onFoot.LMClick.started += ctx => LMClick();
        onFoot.EInteraction.started += ctx => EInteraction();

        onFoot.Enable();
    }
   


    void Update()
    {

         // Check if the current client is the owner of the player object
        if (!IsOwner || !Camera.main)
        {
            // If not the owner, return and do not perform any actions
            return;
        }

        
        bool canPerformAction =  !isAnyUIOpen;
        // Cast a ray from the center of the screen to check if its hitting something
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceToInteract, playerLayer) && canPerformAction){
            isBreakableChecker(canPerformAction, hit);
            collectibleResourceChecker(canPerformAction, hit);
            isInteractable(canPerformAction, hit);
        }else
        {
            //reset when not looking to the right layers
            
            // If the raycast doesn't hit anything
            currentLookingObject = null;

            //collectibleResourceChecker
            if(tooltip.gameObject.activeSelf){
                tooltip.gameObject.SetActive(false); // Deactivate the tooltip
            }


            //isBreakable
            resourceHPBarScript.setActiveHPBar(false); // Deactivate health bar when no hit is detected
            centerUIHandlerScript.setCenterDotColor(Color.white); // Set center dot color to default
            resourceHPBarScript.resetPreviousHealthValue();


            //isInteractable
            if(pressEToInteract.gameObject.activeSelf){
                pressEToInteract.gameObject.SetActive(false);
            }
        }

        if(onFoot.LMClick.IsPressed()){LMClick();}

    }

    //left mouse click
    private void LMClick(){
        //if the inventory is open, or if the player cant click or no equipment equipped, return
        if(!IsOwner || isAnyUIOpen || !canClick || toolHandler.getCurrentEquipment() == null ) {return;}
        canClick = false;
        gameObject.GetComponent<InputMovement>().setIsAttacking(true);

        ResourceGenericHandler getCurrentEquipment = toolHandler.getCurrentEquipment();

        StartCoroutine(ClickCooldown(getCurrentEquipment)); // Start cooldown
        toolHandler.changeAnimationState("Hit");//animate
        //invoke the LMClickHandler function half the time of the debouce
        Invoke(nameof(LMClickHandler), getCurrentEquipment.getDebounceTime() / 2); 
        
        //play sound
    }

    private void LMClickHandler(){
         //this is the rayCasting that will check if at half the couroutine the player was ainming to something

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceToInteract, playerLayer)){
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IsBreakable")){
                
                ResourceHandler resourceHandler = hit.collider.GetComponent<ResourceHandler>();

                if (resourceHandler != null){
                    //do animation
                    //raycast at half the animation (debounce)
                    

                    //damage is given to the isBreakable object
                    ResourceGenericHandler currentTool = toolHandler.getCurrentEquipment();
                    resourceHandler.giveDamage(currentTool.getDamage(), currentTool.getResourceType());
                    resourceHPBarScript.startCoroutines();
                }  
            }
        }
    }

    private void EInteraction(){
        //if the inventory is open, return
        if(!IsOwner || isAnyUIOpen) {return;}

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceToInteract, playerLayer))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("DroppedCollectibleResource")){
                DroppedCollectibleResourceFunction(hit.collider.gameObject); // Collect resource on 'E' key press       
            }else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable")){
                //if the player is looking at an Interactable and clicks E
                if(hit.collider.CompareTag("Blacksmith")){
                    //its the blacksmith
                    npcUiHandler.openOrCloseBlackSmithUI();
                }else if(hit.collider.CompareTag("Dialogue")){
                    //open the dialogue window
                    npcUiHandler.openOrCloseDialogue(hit.collider.gameObject, hit.collider.gameObject.GetComponent<NPCDialogueHandler>().getDialogue());
                }              
            }else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("NativeCollectibleResource")){
                NativeCollectibleResourceFunction(hit.collider.gameObject); // Collect resource on 'E' key press
            }
        }
    }

    // Check for breakable objects
    private void isBreakableChecker(bool canPerformAction, RaycastHit hit)
    {
        if(!canPerformAction) return;
       
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IsBreakable"))
        {
            ResourceHandler resourceHandler = hit.collider.GetComponent<ResourceHandler>();
                
            resourceHPBarScript.setHpBar(resourceHandler); // Update health bar
            centerUIHandlerScript.setCenterDotColor(isBreakableCenterColor);
        }
        else
        {
            resourceHPBarScript.setActiveHPBar(false);// Deactivate health bar for non-breakable objects
        }
       
    }

    // Check for collectible resources
    private void collectibleResourceChecker(bool canPerformAction, RaycastHit hit)
    {
       
        if (!canPerformAction) return;


       
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("DroppedCollectibleResource"))
        {
            centerUIHandlerScript.setCenterDotColor(CollectibleResourceCenterColor); // Set center dot color for collectible resources
            if (currentLookingObject != hit.collider.gameObject)
            {
                // If the player starts looking at a new collectible resource
                DroppedResource droppedResource = hit.collider.gameObject.GetComponent<DroppedResource>();

                currentLookingObject = hit.collider.gameObject;

                if(droppedResource){
                        //its a collectble dropped from a resource
                    setTextTo3DToolTipResourceDropped(hit.collider.gameObject.GetComponent<DroppedResource>()); // Display tooltip for collectible resource
                }

                if(!tooltip.gameObject.activeSelf){
                    tooltip.gameObject.SetActive(true); // Activate the tooltip
                }
            }
        }
        else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("NativeCollectibleResource")){
            centerUIHandlerScript.setCenterDotColor(CollectibleResourceCenterColor); // Set center dot color for collectible resources
            if (currentLookingObject != hit.collider.gameObject)
            {
                    // If the player starts looking at a new collectible resource
                ResourceHandler resourceHandler = hit.collider.gameObject.GetComponent<ResourceHandler>();

                currentLookingObject = hit.collider.gameObject;

                if(resourceHandler){
                        //its a collectble dropped from a resource
                    setTextTo3DToolTipResourceNative(hit.collider.gameObject.GetComponent<ResourceHandler>()); // Display tooltip for collectible resource
                }

                if(!tooltip.gameObject.activeSelf){
                    tooltip.gameObject.SetActive(true); // Activate the tooltip
                }
            }
        }else
        {
            // If the player is not looking at a collectible resource
            currentLookingObject = null;

            if(tooltip.gameObject.activeSelf){
                tooltip.gameObject.SetActive(false); // Deactivate the tooltip
            }
        }
        
        
    }
    
    private void isInteractable(bool canPerformAction, RaycastHit hit){
        if(!canPerformAction) return;

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            centerUIHandlerScript.setCenterDotColor(interactableColor); // Set center dot color for collectible resources

            if(!pressEToInteract.gameObject.activeSelf){
                pressEToInteract.gameObject.SetActive(true);
            }
        }else{
            if(pressEToInteract.gameObject.activeSelf){
                pressEToInteract.gameObject.SetActive(false);
            }
        }
    }

    

    // Coroutine for cooldown of tool actions
    //only weapons, pickaxes and axes can have cooldown
    IEnumerator ClickCooldown(ResourceGenericHandler currentTool)
    {
        canClick = false;
        float cooldownTime = currentTool.getDebounceTime();

        centerUIHandlerScript.startCooldownSlider();

        while (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
            centerUIHandlerScript.setCooldownSlider(cooldownTime / currentTool.getDebounceTime());
            yield return null;
        }
        gameObject.GetComponent<InputMovement>().setIsAttacking(false);
        centerUIHandlerScript.endCooldownSlider();

        // Reset the canClick flag after the cooldown
        canClick = true;
        
    }

    // Block the player from clicking and getting resources while the inventory is open
    public void setIsAnyUIOpen(bool value)
    {
        isAnyUIOpen = value;
    }

    // Collectible resource function to add resources to the player inventory
    private void DroppedCollectibleResourceFunction(GameObject collectibleResource)
    {
        bool collected = false;//debounce

        if (collected == false)
        {
            collected = true;
            // Access the ResourceInventory component of the player and add the resource
            DroppedResource droppedResourceScript = collectibleResource.GetComponent<DroppedResource>();

            //its a dropped resource like a 
            player.GetComponent<ResourceInventory>().AddResource(droppedResourceScript.getResource(), droppedResourceScript.getAmount(), collectibleResource);

        }
    }

    private void NativeCollectibleResourceFunction(GameObject collectibleResource){
        bool collected = false;//debounce

        if (collected == false)
        {
            collected = true;
            // Access the ResourceInventory component of the player and add the resource
            ResourceHandler resourceHandlerScript = collectibleResource.GetComponent<ResourceHandler>();

            //its a dropped resource like a 
            player.GetComponent<ResourceInventory>().AddResource(resourceHandlerScript.getResourceDrop(), resourceHandlerScript.getRandomAmount(), collectibleResource);

        }
    }

 

    // Set text for the 3D tooltip
    private void setTextTo3DToolTipResourceDropped(DroppedResource resource)
    {
        if(resource == null) return;
        string resourceNameText = "<color=#FF9AA2>" + resource.getResource().getName() + "</color>"; // Pastel pink
        string amountText = "<color=#77DD77>" + resource.getAmount().ToString() + "</color>"; // Pastel green
        string weightText = "<color=#FFD700>" + resource.getResource().getWeight().ToString() + "</color>"; // Pastel gold
        string totalWeightText = "<color=#C2B280>" + (resource.getResource().getWeight() * resource.getAmount()).ToString() + "</color>"; // Pastel brown

        this.resourceNameText.text = resourceNameText;
        this.amountText.text = amountText;
        this.weightText.text = weightText;
        this.totalWeightText.text = totalWeightText;
    }

    private void setTextTo3DToolTipResourceNative(ResourceHandler resource){
        if(resource == null) return;
        string resourceNameText = "<color=#FF9AA2>" + resource.getName() + "</color>"; // Pastel pink
        string amountText = "<color=#77DD77>" + resource.getRandomAmount().ToString() + "</color>"; // Pastel green
        string weightText = "<color=#FFD700>" + resource.getResourceDrop().getWeight().ToString() + "</color>"; // Pastel gold
        string totalWeightText = "<color=#C2B280>" + (resource.getResourceDrop().getWeight() * resource.getRandomAmount()).ToString() + "</color>"; // Pastel brown

        this.resourceNameText.text = resourceNameText;
        this.amountText.text = amountText;
        this.weightText.text = weightText;
        this.totalWeightText.text = totalWeightText;
    }

    public void setOnStart( GameObject entirePlayerUIInstance){
        centerUIHandlerScript = entirePlayerUIInstance.GetComponent<CenterUIHandler>();
        resourceHPBarScript = entirePlayerUIInstance.GetComponent<ResourceHPBar>();
        npcUiHandler = entirePlayerUIInstance.GetComponent<NPCUIHandler>();
    }



    
    private void OnDisable() {
        if(!IsOwner) return;
        onFoot.Disable();
    }
}
