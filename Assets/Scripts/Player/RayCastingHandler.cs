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

    [SerializeField] private Color isBreakableCenterColor;
    [SerializeField] private Color CollectibleResourceCenterColor;
    [SerializeField] private Image centerDot;

    [SerializeField] private GameObject hPTopScreen; // Health bar UI
    [SerializeField] private TextMeshProUGUI hpTopScreenText; // Health bar text
    [SerializeField] private Slider hpTopScreenSlider; // Health bar slider
    [SerializeField] private TextMeshProUGUI hpTextAnimation;

    private GameObject activeTooltip;
    [SerializeField] private GameObject tooltipPrefab; // Prefab for the tooltip UI

    private bool canClick = true;
    private bool inventoryIsOpen = false;
    private bool clickedMouse = false; // Flag for left mouse button click

    // Previous health value to track changes
    private float previousHealthValue = float.MinValue;

    void Start()
    {
        cooldownSlider.gameObject.SetActive(false); // Disable cooldown slider at start
    }

    void Update()
    {
        ClearTooltip(); // Clear any active tooltip

        // Cast a ray from the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of the screen
        
        bool canPerformAction = canClick && toolHandler.getCurrentTool() != null;

        // Check if the player is holding down or has pressed the left mouse button
        if (!inventoryIsOpen && canPerformAction && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
        {
            StartCoroutine(ClickCooldown(toolHandler.getCurrentTool())); // Start cooldown
            clickedMouse = true;
        }

        // Check for breakable objects
        isBreakableChecker(ray, canPerformAction);
        
        // Check for collectible resources
        collectibleResourceChecker(ray);

        // Set clickedMouse to false when the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            clickedMouse = false;
        }
    }

    // Check for breakable objects
    private void isBreakableChecker(Ray ray, bool canPerformAction)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceHit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IsBreakable") && !inventoryIsOpen)
            {
                ResourceHandler resourceHandler = hit.collider.GetComponent<ResourceHandler>();
                if (canPerformAction && clickedMouse && hit.distance <= maxDistanceHit)
                {                
                    if (resourceHandler != null)
                    {
                        //damage is given to the isBreakable object
                        ToolGenericHandler currentTool = toolHandler.getCurrentTool();
                        resourceHandler.giveDamage(currentTool.getDamage(), currentTool.getToolType(), player);
                        // Animate hpTextAnimation
                        StartCoroutine(AnimateHPText());
                        // Shake the slider
                        StartCoroutine(ShakeSlider(hpTopScreenSlider));
                        // Briefly change the color to red and then back to white
                        StartCoroutine(ChangeSliderColor(hpTopScreenSlider));
                    }
                }
                setHpBar(resourceHandler); // Update health bar
                centerDot.color = isBreakableCenterColor; // Set center dot color
            }
            else
            {
                hPTopScreen.SetActive(false); // Deactivate health bar for non-breakable objects
            }
        }
        else
        {
            // If not hitting anything
            hPTopScreen.SetActive(false); // Deactivate health bar when no hit is detected
            centerDot.color = Color.white; // Set center dot color to default
            previousHealthValue = 0f;
        }
    }

    // Check for collectible resources
    private void collectibleResourceChecker(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistanceGetDroppedResource))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CollectibleResource") && !inventoryIsOpen)
            {
                centerDot.color = CollectibleResourceCenterColor; // Set center dot color for collectible resources

                if(Input.GetKeyDown(KeyCode.E))
                {
                    CollectibleResourceFunction(hit.collider.gameObject); // Collect resource on 'E' key press
                }
                else
                {
                    DisplayTooltip(hit.collider.gameObject); // Display tooltip for collectible resource
                }
            }
        }
    }

    // Update health bar of breakable objects
    private void setHpBar(ResourceHandler resourceHandler)
    {
        // Activate health bar if not already active
        if (!hPTopScreen.activeSelf)
        {
            hPTopScreen.SetActive(true);
            hpTopScreenText.text = resourceHandler.getName(); // Update health bar text
        }

        // Only update if the health value has changed or the UI becomes visible
        if (hPTopScreen.activeSelf && (float)resourceHandler.getCurrentHealth() != previousHealthValue)
        {
            // Convert double values to float for Slider component
            float maxHealth = (float)resourceHandler.getStartHealth();
            float currentHealth = (float)resourceHandler.getCurrentHealth();

            // Set the values to the Slider
            hpTopScreenSlider.maxValue = maxHealth;
            hpTopScreenSlider.value = currentHealth;

            // Update the previous health value
            previousHealthValue = currentHealth;
        }
    }

    // Coroutine for cooldown of tool actions
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

            player.GetComponent<ResourceInventory>().AddResource(droppedResourceScript.getResource(), droppedResourceScript.getAmount());

            Destroy(collectibleResource); // Destroy the collected resource object
        }
    }

   // Display tooltip for collectible resources
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

        setTextTo3DToolTip(tooltip, droppedResourceScript); // Set text for the tooltip
        // Optional: Update tooltip content (e.g., resource type, quantity, etc.)
    }

    // Clear any active tooltip
    void ClearTooltip()
    {
        Destroy(activeTooltip);
    }

    // Set text for the 3D tooltip
    private void setTextTo3DToolTip(GameObject toolTip, DroppedResource resource)
    {
        TextMeshPro textMeshPro = toolTip.GetComponent<TextMeshPro>();
        if (textMeshPro)
        {
            textMeshPro.text = resource.getResource().getResourceName() + "\n Qty: " + resource.getAmount() + "\n Press E to collect";
        }
    }

    #region hpSlider animations
   // Coroutine to shake the slider
    private IEnumerator ShakeSlider(Slider slider)
    {
        Vector3 originalPosition = slider.transform.position;
        float shakeAmount = 10f; // Increase shake amount
        float shakeDuration = 0.3f; // Increase shake duration
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            slider.transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset slider position
        slider.transform.position = originalPosition;
    }

    // Coroutine to change the color of the slider briefly
    private IEnumerator ChangeSliderColor(Slider slider)
    {
        Color originalColor = slider.fillRect.GetComponent<Image>().color; // Store current color
        
        // Define a softer red color with reduced intensity
        Color softerRedColor = new Color(1f, 0.5f, 0.5f); // Adjust the values as needed
        
        slider.fillRect.GetComponent<Image>().color = softerRedColor; // Change to softer red color

        yield return new WaitForSeconds(0.1f); // Adjust duration as needed

        slider.fillRect.GetComponent<Image>().color = originalColor; // Restore original color
    }

    // Coroutine to animate hpTextAnimation
    private IEnumerator AnimateHPText()
    {
        // Add your animation logic for hpTextAnimation here
        // For example, you can use LeanTween or other animation libraries
        // to make the text animate from the center of the slider
        yield return null;
    }
    #endregion
}
