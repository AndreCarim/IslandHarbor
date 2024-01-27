using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RayCastingHandler : MonoBehaviour
{
    [SerializeField] private float maxDistanceHit = 4.5f;
    [SerializeField] private float maxDistanceGetDroppedResource = 6.5f;
    [SerializeField] private ToolHandler toolHandler;
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject HPTopScreen;

    private GameObject activeTooltip;
    [SerializeField] private GameObject tooltipPrefab; // Prefab for the tooltip UI

    private bool canClick = true;
    private bool inventoryIsOpen = false;
    private bool clickedMouse = false; //this will check if the player clicked the left mouse button 

    void Start()
    {
        cooldownSlider.gameObject.SetActive(false);
    }


    void Update()
    {
        ClearTooltip();

        // Cast a ray from the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 0.5f, 0.5f is the center of the screen
        RaycastHit hit;
        
        bool canPerformAction = canClick && toolHandler.getCurrentTool() != null;

        // Check if the player is holding down or has pressed the left mouse button
        if (!inventoryIsOpen && canPerformAction && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
        {
            StartCoroutine(ClickCooldown(toolHandler.getCurrentTool()));
            clickedMouse = true;
        }

        // Check if the player is looking at a breakable object
       if (Physics.Raycast(ray, out hit, maxDistanceHit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IsBreakable") && !inventoryIsOpen)
            {
                if (canPerformAction && clickedMouse && hit.distance <= maxDistanceHit)
                {
                    ResourceHandler resourceHandler = hit.collider.GetComponent<ResourceHandler>();
                    if (resourceHandler != null)
                    {
                        ToolGenericHandler currentTool = toolHandler.getCurrentTool();
                        resourceHandler.giveDamage(currentTool.getDamage(), currentTool.getToolType(), player);
                    }
                }
                HPTopScreen.SetActive(true); // Activating HPTopScreen only for breakable objects
            }
            else
            {
                HPTopScreen.SetActive(false); // Deactivating HPTopScreen for non-breakable objects
            }
        }
        else
        {
            HPTopScreen.SetActive(false); // Deactivating HPTopScreen when no hit is detected
        }
        
        // Check if the player is looking at a collectible resource and pressing E
        if (Physics.Raycast(ray, out hit, maxDistanceGetDroppedResource))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CollectibleResource") && !inventoryIsOpen)
            {
                if(Input.GetKeyDown(KeyCode.E)){
                    CollectibleResourceFunction(hit.collider.gameObject);
                }else{
                    DisplayTooltip(hit.collider.gameObject);
                }
            }
        }

        // Set clickedMouse to false when the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            clickedMouse = false;
        }
    }




    IEnumerator ClickCooldown(ToolGenericHandler currentTool)
    {
        canClick = false;
        float cooldownTime = currentTool.getDebounceTime();

        // Enable the slider before starting the cooldown
        cooldownSlider.gameObject.SetActive(true);
        cooldownSlider.value = 1f;

        while (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
            cooldownSlider.value = cooldownTime / currentTool.getDebounceTime();
            yield return null;
        }

        // Ensure the slider is set to 0 when the cooldown is complete
        cooldownSlider.value = 0f;

        // Disable the slider after the cooldown
        cooldownSlider.gameObject.SetActive(false);

        // Reset the canClick flag after the cooldown
        canClick = true;
    }

    //block the player from clicking and getting resources while the inventory is open
    public void setInventoryIsOpen(bool value)
    {
        inventoryIsOpen = value;
    }

    private void CollectibleResourceFunction(GameObject collectibleResource)
    {
       bool collected = false;

       if(collected == false){
            collected = true;
            // Access the ResourceInventory component of the player and add the resource
            DroppedResource droppedResourceScript = collectibleResource.GetComponent<DroppedResource>();
            
            player.GetComponent<ResourceInventory>().AddResource(droppedResourceScript.getResource(), droppedResourceScript.getAmount());

            Destroy(collectibleResource);
       }
   
    }

    void DisplayTooltip(GameObject resourceObject)
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

        setTextTo3DToolTip(tooltip, droppedResourceScript);
        // Optional: Update tooltip content (e.g., resource type, quantity, etc.)
    }

    void ClearTooltip()
    {
        Destroy(activeTooltip);
    }

     private void setTextTo3DToolTip(GameObject toolTip, DroppedResource resource){
         TextMeshPro textMeshPro = toolTip.GetComponent<TextMeshPro>();
         if(textMeshPro){
            textMeshPro.text = resource.getResource().getResourceName() + "\n Qty: " + resource.getAmount() + "\n Press E to collect";
         }
    }
}
