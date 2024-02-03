using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class InventoryUIHandler : NetworkBehaviour
{
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
    [SerializeField] private TextMeshProUGUI amountWeightDropText;

    [SerializeField] private GameObject toolTip;
    [SerializeField] private RectTransform canvasRectTransform;

    [SerializeField] private Slider weightSlider;
    [SerializeField] private TextMeshProUGUI currentMaxCarryWeightText;
    [SerializeField] private TextMeshProUGUI currentCarryWeightText;

    private GameObject player;

    private int currentMaxCarryWeight;
    private int currentCarryWeight;

    private ResourceGenericHandler resourceSelected;
    private GameObject slot;

    private Dictionary<int, GameObject> resourceUIDictionary = new Dictionary<int, GameObject>();

     // Store the Coroutine reference for the shaking animation
    private Coroutine shakeCoroutine;

    public bool checkActiveInventory(){
    if(!IsOwner){return false;}

        if(inventoryUI.activeSelf){
            resourceSelected = null;
            slot = null;
        }

       bool isOpen = !inventoryUI.activeSelf;
       inventoryUI.SetActive(isOpen);

       

       return isOpen;
    }    

    public void setweightText(){
        if(!IsOwner){return;}

        weightSlider.maxValue = (float)currentMaxCarryWeight;
        weightSlider.value = (float)currentCarryWeight;   

        currentCarryWeightText.text = currentCarryWeight.ToString();
        currentMaxCarryWeightText.text = currentMaxCarryWeight.ToString();
    }

    public void CreateOrUpdateResourceUI(ResourceGenericHandler resource, int amount)
    {
        if(!IsOwner){return;}

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
            newResourceUI.GetComponent<SlotHandlerInventory>().setOnCreate(toolTip, canvasRectTransform, resource, player);

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
            setweightText();
        }
    }

    // Method to update UI for a resource
    private void UpdateResourceUI(GameObject resourceUI, int amount)
    {
        if(!IsOwner){return;}
        TextMeshProUGUI amountText = resourceUI.GetComponentInChildren<TextMeshProUGUI>();

        // Set the amount text
        amountText.text = amount.ToString();

        // Disable the UI if the amount is 0
        resourceUI.SetActive(amount > 0);
    }

     // Method to remove UI for a resource
    public void RemoveResourceUI(int resourceId)
    {
        if(!IsOwner){return;}

        if (resourceUIDictionary.ContainsKey(resourceId))
        {
            GameObject resourceUI = resourceUIDictionary[resourceId];
            resourceUIDictionary.Remove(resourceId);
            Destroy(resourceUI);
        }
    }

    public void setItemInformation(ResourceGenericHandler resourceSelected, bool contains, int maxValue = 0){
        if(!IsOwner){return;}

        if(contains){ //checks if the player has the item
            resourceInfo.text = resourceSelected.getInformationText();
            resourceName.text = resourceSelected.getResourceName();
            resourceIcon.sprite = resourceSelected.getIcon();

            amountToDropSlider.maxValue = maxValue;

            if(maxValue > 1){
                amountToDropSlider.minValue = 1f;
                amountToDropSlider.value = 1f;
            }else{
                amountToDropSlider.minValue = 0f;
                amountToDropSlider.value = 1f;
            }
    
            
        }else{
            resourceInforUI.SetActive(false);
        }   

        setTextAmountDrop();
    }

    public void setTextAmountDrop(){
        if(!IsOwner){return;}

        amountDropText.text = amountToDropSlider.value.ToString();
        setweightText();

        if(resourceSelected){
            amountWeightDropText.text = (resourceSelected.getWeight() * amountToDropSlider.value).ToString();
        }

        if(amountToDropSlider.value > 0){
            dropButton.SetActive(true);
        }else{
            dropButton.SetActive(false);
        }
    }

    

    public void setMaxDrop(){
        if(!IsOwner){return;}

        amountToDropSlider.value = amountToDropSlider.maxValue;
        setTextAmountDrop();
    }

    public void setMinDrop(){
        if(!IsOwner){return;}

        amountToDropSlider.value = amountToDropSlider.minValue;
        setTextAmountDrop();
    }

    public void setUIInfo(bool value){
        if(!IsOwner){return;}

        resourceInforUI.SetActive(value);
    }

    public void setResourceSelected(ResourceGenericHandler resourceSelected, GameObject slot, int amount){
        if(!IsOwner){return;}

         if (this.slot != slot)
        {
            this.resourceSelected = resourceSelected;
            this.slot = slot;

            // Start shaking the slot
            if (slot != null)
            {
                StartShaking();
                setItemInformation(resourceSelected, true, amount);
            }

            resourceInforUI.SetActive(true);
        }
    }

    public void dropButtonPressed(){
        if(!IsOwner){return;}

        player.GetComponent<ResourceInventory>().dropButtonPressed();
    }

    // Function to start the shake animation for the slot
    public void StartShaking()
    {
        if(!IsOwner){return;}
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

    public void stopCoroutine(){
        if(!IsOwner){return;}

        if (shakeCoroutine != null){
            StopCoroutine(shakeCoroutine);
        }       
    }

    public void setWeight(int currentCarryWeight, int currentMaxCarryWeight){
        if(!IsOwner){return;}

        this.currentCarryWeight = currentCarryWeight;
        this.currentMaxCarryWeight = currentMaxCarryWeight;

        setweightText();
    }

    public void setToolTip(bool value){
        if(!IsOwner){return;}

        toolTip.SetActive(value);
    }

    public void setPlayer(GameObject player){
        if(!IsOwner){return;}

        this.player = player;
    }

    public int getAmountToDropSlider(){
        if(!IsOwner){return 0;}

        return (int)amountToDropSlider.value;
    }


}
