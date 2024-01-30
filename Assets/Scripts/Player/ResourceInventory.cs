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
    [SerializeField] private GameObject resourceInforUI;

    //references for the information of the items
    [SerializeField] private TextMeshProUGUI resourceName;
    [SerializeField] private TextMeshProUGUI resourceInfo;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Slider amountToDropSlider;
    [SerializeField] private TextMeshProUGUI amountDropText;
 
    private Dictionary<int, GameObject> resourceUIDictionary = new Dictionary<int, GameObject>();

    [SerializeField] private FirstPersonCameraHandler cameraScript;
    [SerializeField] private Movement movementScript;
    [SerializeField] private RayCastingHandler clickScript;

    [SerializeField] private GameObject toolTip;
    [SerializeField] private RectTransform canvasRectTransform;

    private ResourceGenericHandler resourceSelected;
    private GameObject slot;
    private Camera mainCamera;

    // Store the Coroutine reference for the shaking animation
    private Coroutine shakeCoroutine;

    void Start(){
        mainCamera = Camera.main;
    }

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
                resourceInforUI.SetActive(false);
                slot = null;
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


    


    public void slotSelected(ResourceGenericHandler resourceSelected, GameObject slot){
    // Check if the selected slot is different from the current one
        if (this.slot != slot)
        {
            this.resourceSelected = resourceSelected;
            this.slot = slot;

            // Start shaking the slot
            if (slot != null)
            {
                StartShaking();
                setItemInformation(resourceSelected);
            }

            resourceInforUI.SetActive(true);
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

    //this is being called by the button component on the dropButton isnde the inventory UI
    public void dropButtonPressed()
    {
        // Check if the resource is in the inventory
        if (resourceUIDictionary.ContainsKey(resourceSelected.getId()))
        {
            // Get the amount of the selected resource
            int amountPlayerHas = resourceCountDictionary.ContainsKey(resourceSelected.getId()) ? resourceCountDictionary[resourceSelected.getId()] : 0;

            // Check if the player has enough resource
            int amountToDrop = (int)amountToDropSlider.value;
            if (amountPlayerHas >= amountToDrop)
            {
                // Stop the previous shaking coroutine if it's running
                if (shakeCoroutine != null)
                {
                    StopCoroutine(shakeCoroutine);
                }

                // If the player has only one, drop imidiatly
                RemoveResource(resourceSelected, amountToDrop);

                // Calculate the center of the GameObject
                Vector3 positionToDrop = calculatePositionToDrop();

               // Instantiate the item at the center of the original GameObject
                GameObject droppedItem = Instantiate(resourceSelected.getDropGameObject(), positionToDrop, Quaternion.identity);
                droppedItem.AddComponent<DroppedResource>();

                droppedItem.GetComponent<DroppedResource>().setResource(amountToDrop, resourceSelected);

                // Change the layer to "ResourceFloating"
                droppedItem.layer = LayerMask.NameToLayer("CollectibleResource");

                setItemInformation(resourceSelected);
            }
        }
        else
        {
            // If the resource is not in the inventory, do nothing. THIS SHOULD NOT HAPPEN!
            Debug.Log("How are you clicking on a item that you dont have? \n and why are you seeing this message???");
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

    private void setItemInformation(ResourceGenericHandler resourceSelected){
        if(resourceCountDictionary.ContainsKey(resourceSelected.getId())){ //checks if the player has the item
            resourceInfo.text = resourceSelected.getInformationText();
            resourceName.text = resourceSelected.getResourceName();
            resourceIcon.sprite = resourceSelected.getIcon();

            amountToDropSlider.maxValue = resourceCountDictionary[resourceSelected.getId()];

            if(resourceCountDictionary[resourceSelected.getId()] > 1){
                amountToDropSlider.minValue = 1f;
                amountToDropSlider.value = 1f;
            }else{
                amountToDropSlider.minValue = 0f;
                amountToDropSlider.value = 1f;
            }

            setTextAmountDrop();
        }else{
            resourceInforUI.SetActive(false);
        }
        
    }

    public void setTextAmountDrop(){
        amountDropText.text = amountToDropSlider.value.ToString();

        if(amountToDropSlider.value > 0){
            dropButton.SetActive(true);
        }else{
            dropButton.SetActive(false);
        }
    }

    public void setMaxDrop(){
        amountToDropSlider.value = amountToDropSlider.maxValue;
        setTextAmountDrop();
    }

    public void setMinDrop(){
        amountToDropSlider.value = amountToDropSlider.minValue;
        setTextAmountDrop();
    }
}
