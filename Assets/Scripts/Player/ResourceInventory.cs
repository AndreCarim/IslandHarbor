using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceInventory : MonoBehaviour
{
    // Dictionary to store resource counts with their IDs as keys
    [SerializeField] private Dictionary<int, int> resourceCountDictionary = new Dictionary<int, int>();

    // Property to expose the resource count dictionary (read-only)
    public Dictionary<int, int> ResourceCountDictionary => resourceCountDictionary;

    [SerializeField] private GameObject resourceUIPrefab;
    [SerializeField] private Transform resourceUIParent;

    private Dictionary<int, GameObject> resourceUIDictionary = new Dictionary<int, GameObject>();

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

        Debug.Log(resourceCountDictionary[resource.getId()]);
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
}
