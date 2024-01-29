using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceInventory : MonoBehaviour
{
    // This dictionary will store the item ID as the key and the quantity as the value.
    [SerializeField] private Dictionary<int, int> resourceCountDictionary = new Dictionary<int, int>();

    //  This dictionary will store the item ID as the key and the corresponding UI GameObject as the value.
    public Dictionary<int, int> ResourceCountDictionary => resourceCountDictionary;

    [SerializeField] private GameObject resourceUIPrefab;
    [SerializeField] private Transform resourceUIParent;

    [SerializeField] private GameObject inventoryUI; // Reference to the inventory UI panel
    [SerializeField] private GameObject dropButton;
    [SerializeField] private GameObject howManyToDropUI;
 
    private Dictionary<int, GameObject> resourceUIDictionary = new Dictionary<int, GameObject>();

    [SerializeField] private FirstPersonCameraHandler cameraScript;
    [SerializeField] private Movement movementScript;
    [SerializeField] private RayCastingHandler clickScript;

    [SerializeField] private GameObject toolTip;
    [SerializeField] private RectTransform canvasRectTransform;

    private ResourceGenericHandler resourceSelected;
    private GameObject slot;

    // Store the Coroutine reference for the shaking animation
    private Coroutine shakeCoroutine;

    void Update()
    {
        // Check if the player presses the Tab or I key
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            // Toggle the visibility of the inventory UI panel
            bool isOpen = !inventoryUI.activeSelf;
            inventoryUI.SetActive(isOpen);


             // Call different functions based on the inventory state
            if (isOpen)
            {
                cameraScript.setIsFreeToLook(false); //lock the mouse
                movementScript.setCanWalk(false); // lock walking
                clickScript.setInventoryIsOpen(true); // lock clicking
                dropButton.SetActive(false);
            }
            else
            {
                cameraScript.setIsFreeToLook(true); // unlock the mouse
                movementScript.setCanWalk(true); // unlock the walk
                clickScript.setInventoryIsOpen(false); // unlock clicking
                toolTip.SetActive(false);
            }
        }
    }

    // Method to add a resource to the inventory
    public void AddResource(ResourceGenericHandler resource, int amount)
    {
        // Check if the resource ID already exists in the dictionary
        if (resourceCountDictionary.ContainsKey(resource.getId()))
        {
            // If it exists, increment its count
            resourceCountDictionary[resource.getId()] += amount;
        }
        else
        {
            // If it doesn't exist, add it to the dictionary with its count
            resourceCountDictionary.Add(resource.getId(), amount);
        }

        // Create UI for the new resource
        CreateOrUpdateResourceUI(resource, resourceCountDictionary[resource.getId()]);
    }

    // Method to remove a resource from the inventory
    public void RemoveResource(ResourceGenericHandler resource, int amount)
    {
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
                RemoveResourceUI(resource.getId());
            }
            else
            {
                // Update UI for the resource
                CreateOrUpdateResourceUI(resource, resourceCountDictionary[resource.getId()]);
            }
        }
    }

    //UI

    private void CreateOrUpdateResourceUI(ResourceGenericHandler resource, int amount)
    {
        if (resourceUIDictionary.ContainsKey(resource.getId()))
        {
            // Update existing UI
            GameObject resourceUI = resourceUIDictionary[resource.getId()];
            UpdateResourceUI(resourceUI,  amount);
        }
        else
        {
            // Create new UI
            GameObject newResourceUI = Instantiate(resourceUIPrefab, resourceUIParent);
            newResourceUI.name = resource.getId().ToString(); // Set the name to the resource ID
            resourceUIDictionary.Add(resource.getId(), newResourceUI);

            // Update UI elements (image, text) based on resource data
            GameObject iconObject = newResourceUI.transform.Find("Icon").gameObject; // Assuming "icon" is the name of the child object
            Image iconImage = iconObject.GetComponent<Image>();
            newResourceUI.GetComponent<SlotHandlerInventory>().setOnCreate(toolTip, canvasRectTransform, resource, gameObject);

            // Debug: Check if the sprite is not null
            if (resource.getIcon() != null)
            {
                iconImage.sprite = resource.getIcon();
            }
            else
            {
                Debug.LogWarning($"Sprite for resource {resource.getId()} is null");
            }

            UpdateResourceUI(newResourceUI, amount);
        }
    }

    // Method to update UI for a resource
    private void UpdateResourceUI(GameObject resourceUI, int amount)
    {
        TextMeshProUGUI amountText = resourceUI.GetComponentInChildren<TextMeshProUGUI>();

        // Set the amount text
        amountText.text = amount.ToString();

        // Disable the UI if the amount is 0
        resourceUI.SetActive(amount > 0);
    }

    // Method to remove UI for a resource
    private void RemoveResourceUI(int resourceId)
    {
        if (resourceUIDictionary.ContainsKey(resourceId))
        {
            GameObject resourceUI = resourceUIDictionary[resourceId];
            resourceUIDictionary.Remove(resourceId);
            Destroy(resourceUI);
        }
    }

//this is being called by slotHandlerInventory script everytime the player selects a slot
    public void slotSelected(ResourceGenericHandler resourceSelected, GameObject slot){
        this.resourceSelected = resourceSelected;
        this.slot = slot;

        // Start shaking the slot
        if (slot != null)
        {
            StartShaking();
        }

        dropButton.SetActive(true);
    }

    //this is being called by the button component on the dropButton isnde the inventory UI
    public void dropButtonPressed()
    {
        // Check if the resource is in the inventory
        if (resourceUIDictionary.ContainsKey(resourceSelected.getId()))
        {
            // Get the amount of the selected resource
            int amount = resourceCountDictionary.ContainsKey(resourceSelected.getId()) ? resourceCountDictionary[resourceSelected.getId()] : 0;

            // Check if the player has more than one of this resource
            if (amount > 1)
            {
                // If the player has more than one, we want to ask how many we want to drop
                Debug.Log("Player has more than one of this resource. Implement dropping logic.");
            }
            else
            {
                // If the player has only one, drop imidiatly
                Debug.Log("Player has only one of this resource. Implement handling logic.");
            }
        }
        else
        {
            // If the resource is not in the inventory, do nothing. THIS SHOULD NOT HAPPEN!
            Debug.Log("How are you clicking on a item that you dont have? \n and why are you seeing this message???");
        }
    }


    // Function to start the shake animation for the slot
    public void StartShaking()
    {
        // Stop the previous shaking coroutine if it's running
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        // Start a new coroutine for the current button
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    // Coroutine for the shake animation
    private IEnumerator ShakeCoroutine()
    {
        RectTransform rectTransform = slot.GetComponent<RectTransform>();

        // Intensity of the shake
        float intensity = 5f;

        // Initial position of the slot
        Vector3 originalPosition = rectTransform.anchoredPosition;

        // Shake animation loops
        while (inventoryUI.activeSelf)
        {
            // Calculate a random offset for the shake
            Vector3 randomOffset = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0f);

            // Apply the offset to the slot's position
            rectTransform.anchoredPosition = originalPosition + randomOffset;

            // Wait for the end of the frame
            yield return null;
        }

        // Reset the slot's position to its original position
        rectTransform.anchoredPosition = originalPosition;
    }
}
