using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class InventoryUIHandler : MonoBehaviour
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

    [SerializeField] private TextMeshProUGUI damageValueText;
    [SerializeField] private TextMeshProUGUI debounceValueText;
    [SerializeField] private GameObject statsTabUI;

    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject unequipButton;

    [SerializeField] private GameObject axeEquippedSlot;
    [SerializeField] private GameObject pickAxeEquippedSlot;
    [SerializeField] private GameObject weaponEquippedSlot;
    [SerializeField] private TextMeshProUGUI clockTimer;
    [SerializeField] private TextMeshProUGUI currentDayCountText;

    private GameObject player;

    private int currentMaxCarryWeight;
    private int currentCarryWeight;

    private ResourceGenericHandler resourceSelected;
    private GameObject slot;

    private Dictionary<int, GameObject> resourceUIDictionary = new Dictionary<int, GameObject>();

     // Store the Coroutine reference for the shaking animation
    private Coroutine shakeCoroutine;
     // Initial position of the slot
    private Vector3 originalPosition;


    void Update(){
        //if(!IsOwner) return;

        if(inventoryUI.activeSelf)setClock();
    }

    // Method to check if the player's inventory is currently active
    public bool checkActiveInventory()
    {
       // if(!IsOwner)return;

        // If the inventory is currently active, reset the selected resource and slot
        //and close everything
        if (inventoryUI.activeSelf)
        {
            resourceSelected = null;
            slot = null;
        }


        // Determine whether the inventory is open or closed
        bool isOpen = !inventoryUI.activeSelf;

        

        // Set the inventory UI active state based on whether it's open or closed
        inventoryUI.SetActive(isOpen);

        // Return the status of the inventory (open or closed)
        return isOpen;
    }   

    public void setweightText(){
        //if(!IsOwner){return;}

        weightSlider.maxValue = (float)currentMaxCarryWeight;
        weightSlider.value = (float)currentCarryWeight;   

        currentCarryWeightText.text = currentCarryWeight.ToString();
        currentMaxCarryWeightText.text = currentMaxCarryWeight.ToString();
    }

    public void CreateOrUpdateResourceUI(ResourceGenericHandler resource, int amount)
    {
        //if(!IsOwner){return;}

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
        //if(!IsOwner){return;}
        TextMeshProUGUI amountText = resourceUI.GetComponentInChildren<TextMeshProUGUI>();

        // Set the amount text
        amountText.text = amount.ToString();

        // Disable the UI if the amount is 0
        resourceUI.SetActive(amount > 0);
    }

     // Method to remove UI for a resource
    public void RemoveResourceUI(int resourceId)
    {
        //if(!IsOwner){return;}

        if (resourceUIDictionary.ContainsKey(resourceId))
        {
            GameObject resourceUI = resourceUIDictionary[resourceId];
            resourceUIDictionary.Remove(resourceId);
            Destroy(resourceUI);
        }
    }

    public void setItemInformation(ResourceGenericHandler resourceSelected, bool contains, int maxValue = 0, bool isEquipped = false){
        //if(!IsOwner){return;}

        if(contains){ //checks if the player has the item
            resourceInfo.text = resourceSelected.getInformationText();
            resourceName.text = resourceSelected.getName();
            resourceIcon.sprite = resourceSelected.getIcon();

            amountToDropSlider.maxValue = maxValue;

            if(maxValue > 1){
                amountToDropSlider.minValue = 1f;
                amountToDropSlider.value = 1f;
            }else{
                amountToDropSlider.minValue = 0f;
                amountToDropSlider.value = 1f;
            }

            if(resourceSelected.getResourceType() == ResourceEnum.ResourceType.Collectible){
                //if it is a collectible
                equipButton.SetActive(false);
                statsTabUI.SetActive(false);
            }else{
                //if it is a equipment
                equipButton.SetActive(true);
                statsTabUI.SetActive(true);

                setStatsUI(resourceSelected);
            }

            //this will check if the button pressed is one of the already equipped equipments
            if(isEquipped){
                equipButton.SetActive(false);
                unequipButton.SetActive(true);
            }else if(resourceSelected == player.GetComponent<ToolHandler>().getCurrentWeapon()){
                equipButton.SetActive(false);
            }else if(resourceSelected == player.GetComponent<ToolHandler>().getCurrentPickAxe()){
                equipButton.SetActive(false);
            }else if(resourceSelected == player.GetComponent<ToolHandler>().getCurrentPickAxe()){
                equipButton.SetActive(false);
            }else{
                unequipButton.SetActive(false);
            }
    
            
        }else{
            resourceInforUI.SetActive(false);
        }   

        setTextAmountDrop();
    }

    private void setStatsUI(ResourceGenericHandler equipmentSelected){
        damageValueText.text = equipmentSelected.getDamage().ToString();
        debounceValueText.text = equipmentSelected.getDebounceTime().ToString();
    }

    public void setTextAmountDrop(){
        //if(!IsOwner){return;}

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
       // if(!IsOwner){return;}

        amountToDropSlider.value = amountToDropSlider.maxValue;
        setTextAmountDrop();
    }

    public void setMinDrop(){
       // if(!IsOwner){return;}

        amountToDropSlider.value = amountToDropSlider.minValue;
        setTextAmountDrop();
    }

    public void setUIInfo(bool value){
       // if(!IsOwner){return;}

        resourceInforUI.SetActive(value);
    }

    public void setStatsUI(bool value){
       // if(!IsOwner)return;

        statsTabUI.SetActive(value);
    }

    public void setResourceSelected(ResourceGenericHandler resourceSelected, GameObject slot, int amount, bool isEquipped = false){
       // if(!IsOwner){return;}

         if (this.slot != slot)
        {
            this.resourceSelected = resourceSelected;
            this.slot = slot;

            // Start shaking the slot
            if (slot != null)
            {
                StartShaking();
                setItemInformation(resourceSelected, true, amount, isEquipped);
            }

            resourceInforUI.SetActive(true);
        }
    }

    public void dropButtonPressed(){
       // if(!IsOwner){return;}

        player.GetComponent<ResourceInventory>().dropButtonPressed();
    }

    public void equipButtonPressed(){
       // if(!IsOwner) return;

        player.GetComponent<ResourceInventory>().equipButtonPressed();
    }

    public void unequipButtonPressed(){
        player.GetComponent<ResourceInventory>().unequipButtonPressed();
    }

    public void equippedWeaponPressed(){
        if(!player.GetComponent<ToolHandler>().getCurrentWeapon()) return;

        player.GetComponent<ResourceInventory>().slotSelected(player.GetComponent<ToolHandler>().getCurrentWeapon(), weaponEquippedSlot, true);
    }

    public void equippedAxePressed(){
        if(!player.GetComponent<ToolHandler>().getCurrentAxe()) return;

        player.GetComponent<ResourceInventory>().slotSelected(player.GetComponent<ToolHandler>().getCurrentAxe(), axeEquippedSlot, true);
    }

    public void equippedPickAxePressed(){
        if(!player.GetComponent<ToolHandler>().getCurrentPickAxe()) return;

        player.GetComponent<ResourceInventory>().slotSelected(player.GetComponent<ToolHandler>().getCurrentPickAxe(), pickAxeEquippedSlot, true);
    }

    // Function to start the shake animation for the slot
    public void StartShaking()
    {
       // if(!IsOwner){return;}
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
        float intensity = 10f;
        originalPosition = rectTransform.anchoredPosition;
       

        // Shake animation loops
        while (inventoryUI.activeSelf)
        {

           // Calculate a random offset for the shake
            Vector3 randomOffset = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0f);

            if(rectTransform){
                // Apply the offset to the slot's position
                rectTransform.anchoredPosition = originalPosition + randomOffset;
            }else{
                rectTransform.anchoredPosition = originalPosition;
                StopCoroutine(shakeCoroutine);           
            }
            // Wait for the end of the frame
            yield return null;
        }
        // Reset the slot's position to its original position
        rectTransform.anchoredPosition = originalPosition;
    }

    public void stopCoroutine(){
       // if(!IsOwner){return;}

        if (shakeCoroutine != null){
            StopCoroutine(shakeCoroutine);
        }       

        
    }

    public void setWeight(int currentCarryWeight, int currentMaxCarryWeight){
      //  if(!IsOwner){return;}

        this.currentCarryWeight = currentCarryWeight;
        this.currentMaxCarryWeight = currentMaxCarryWeight;

        setweightText();
    }

    public void setToolTip(bool value){
       // if(!IsOwner){return;}

        toolTip.SetActive(value);
    }

    public void setPlayer(GameObject player){
        //if(!IsOwner){return;}

        this.player = player;
    }

    public int getAmountToDropSlider(){
       // if(!IsOwner){return 0;}

        return (int)amountToDropSlider.value;
    }

    public void setSlotNull(){
        slot = null;
        resourceSelected = null;
    }

    private void setClock(){
         // Find the DayCycle GameObject in the scene
        GameObject dayCycleObject = GameObject.Find("DayCycle"); // Replace "DayCycle" with the actual name of your GameObject
        
        // Check if the GameObject is found
        if(dayCycleObject != null){
            // Get the DayCycleHandler component attached to the GameObject
            DayCycleHandler dayCycleHandler = dayCycleObject.GetComponent<DayCycleHandler>();
            
            // Check if the DayCycleHandler component is found
            if(dayCycleHandler != null){
                // Call the getTime method
                string currentTime = dayCycleHandler.getTime();
                clockTimer.text = currentTime;

                string dayCountText = "Day " + dayCycleHandler.getCurrentDay().ToString(); 
                currentDayCountText.text = dayCountText;
            }
            else{
                Debug.LogWarning("DayCycleHandler component not found on DayCycle GameObject.");
            }
        }
        else{
            Debug.LogWarning("DayCycle GameObject not found in the scene.");
        }
    }


}
