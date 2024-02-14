using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class RayCastingHandler : NetworkBehaviour
{
    private CenterUIHandler centerUIHandlerScript;
    private ResourceHPBar resourceHPBarScript;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    [SerializeField] private float maxDistanceHit = 4.5f;
    [SerializeField] private float maxDistanceGetDroppedResource = 6.5f;
    [SerializeField] private ToolHandler toolHandler;
    [SerializeField] private GameObject player;

    [SerializeField] private Color isBreakableCenterColor = new Color(0.9529f, 0.6f, 0.6f); // Assigning a light red color
    [SerializeField] private Color CollectibleResourceCenterColor = new Color(0.6039f, 0.9804f, 0.6745f); // Assigning a color close to #9FFAAC


    private GameObject activeTooltip;
    [SerializeField] private GameObject tooltipPrefab; // Prefab for the tooltip UI

    private bool canClick = true;
    private bool inventoryIsOpen = false;


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

        ClearTooltip(); // Clear any active tooltip


        bool canPerformAction =  !inventoryIsOpen;
        // Cast a ray from the center of the screen to check if its hitting something
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of the screen

         // Change Color and show hp if needed
        isBreakableChecker(canPerformAction);
        collectibleResourceChecker(canPerformAction);

        if(onFoot.LMClick.IsPressed()){LMClick();}

    }

    //left mouse click
    private void LMClick(){
        //if the inventory is open, or if the player cant click or no equipment equipped, return
        if(!IsOwner || inventoryIsOpen || !canClick || toolHandler.getCurrentEquipment() == null ) {return;}
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
        Ray rayHit = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of the screen

        RaycastHit hit;

        if (Physics.Raycast(rayHit, out hit, maxDistanceHit)){
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
        if(!IsOwner || inventoryIsOpen) {return;}

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceGetDroppedResource))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CollectibleResource")){
                 CollectibleResourceFunction(hit.collider.gameObject); // Collect resource on 'E' key press
            }
        }
    }

    // Check for breakable objects
    private void isBreakableChecker(bool canPerformAction)
    {
        if(!canPerformAction) return;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceHit) )
        {
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
        else
        {
            // If not hitting anything
            resourceHPBarScript.setActiveHPBar(false); // Deactivate health bar when no hit is detected
            centerUIHandlerScript.setCenterDotColor(Color.white); // Set center dot color to default
            resourceHPBarScript.resetPreviousHealthValue();
        }
    }

    // Check for collectible resources
    private void collectibleResourceChecker(bool canPerformAction)
    {
        if(!canPerformAction) return;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceGetDroppedResource))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CollectibleResource"))
            {
                centerUIHandlerScript.setCenterDotColor(CollectibleResourceCenterColor); // Set center dot color for collectible resources

                DisplayTooltipResource(hit.collider.gameObject); // Display tooltip for collectible resource

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
    public void setInventoryIsOpen(bool value)
    {
        inventoryIsOpen = value;
    }

    // Collectible resource function to add resources to the player inventory
    private void CollectibleResourceFunction(GameObject collectibleResource)
    {
        bool collected = false;//debounce

        if (collected == false)
        {
            collected = true;
            // Access the ResourceInventory component of the player and add the resource
            DroppedResource droppedResourceScript = collectibleResource.GetComponent<DroppedResource>();

            player.GetComponent<ResourceInventory>().AddResource(droppedResourceScript.getResource(), droppedResourceScript.getAmount(), collectibleResource);
        }
    }

   // Display tooltip for collectible resources
    void DisplayTooltipResource(GameObject resourceObject)
    {
        DroppedResource droppedResourceScript = resourceObject.GetComponent<DroppedResource>();

        // Instantiate the tooltip UI prefab
        GameObject tooltip = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity);
        tooltip.SetActive(true);
        activeTooltip = tooltip;

        // Update tooltip position to be above the resourceFloating object
        tooltip.transform.position = resourceObject.transform.position + Vector3.up * 2f;

        // Calculate the direction from the resourceFloating object to the player
        Vector3 directionToPlayer = (transform.position - resourceObject.transform.position).normalized;

        // Calculate the rotation to face the player
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Apply the rotation, taking into account the current rotation of the tooltip
        tooltip.transform.rotation = rotationToPlayer * Quaternion.Euler(0, 180, 0); // Adjust the 180 degrees to correct the rotation

        setTextTo3DToolTipResource(tooltip, droppedResourceScript); // Set text for the tooltip
        // Optional: Update tooltip content (e.g., resource type, quantity, etc.)
    }

    // Clear any active tooltip
    void ClearTooltip()
    {
        Destroy(activeTooltip);
    }

    // Set text for the 3D tooltip
    private void setTextTo3DToolTipResource(GameObject toolTip, DroppedResource resource)
    {
        TextMeshPro textMeshPro = toolTip.GetComponent<TextMeshPro>();
        if (textMeshPro)
        {
            string resourceName = "<color=red>" + resource.getResource().getName() + "</color>";
            string amountText = "<color=green>Qty: " + resource.getAmount() + "</color>";
            string weightText = "<color=white>" + resource.getResource().getWeight().ToString() + "/" + (resource.getResource().getWeight() * resource.getAmount()).ToString()+ "</color>";
            string instructionText = "<color=blue>Press E to collect</color>";
            
            textMeshPro.text = resourceName + "\n" + amountText + "\n" + weightText + "\n" + instructionText;
        }
    }

    public void setOnStart( GameObject entirePlayerUIInstance){
    

        centerUIHandlerScript = entirePlayerUIInstance.GetComponent<CenterUIHandler>();
        resourceHPBarScript = entirePlayerUIInstance.GetComponent<ResourceHPBar>();
    }
}
