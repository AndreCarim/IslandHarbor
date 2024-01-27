using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//this script is not being currently used.
//to prevent some lag
public class ProximityCheck : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 6f; // Radius within which the player can detect resourceFloating objects
    private LayerMask resourceFloatingLayer; // Layer mask for resourceFloating objects
    [SerializeField] private GameObject tooltipPrefab; // Prefab for the tooltip UI

    private List<GameObject> activeTooltips = new List<GameObject>(); // List to store active tooltips

    [SerializeField] private int maxTooltips = 5; // Maximum number of tooltips allowed
    private int currentTooltips = 0; // Current number of active tooltips

    void Start()
    {
        resourceFloatingLayer = LayerMask.GetMask("CollectibleResource");
    }

    void Update()
    {
        // Check for nearby resourceFloating objects
        Collider[] nearbyResources = Physics.OverlapSphere(transform.position, detectionRadius, resourceFloatingLayer);

        // Clear existing tooltips
        ClearTooltips();

        // Display tooltips for nearby resourceFloating objects up to the maximum limit
        foreach (Collider resource in nearbyResources)
        {
            if (currentTooltips < maxTooltips)
            {
                DisplayTooltip(resource.gameObject);
                currentTooltips++;
            }
            else
            {
                break; // Stop instantiating tooltips if the maximum limit is reached
            }
        }
    }

    void DisplayTooltip(GameObject resourceObject)
    {
        DroppedResource droppedResourceScript = resourceObject.GetComponent<DroppedResource>();

        // Instantiate the tooltip UI prefab
        GameObject tooltip = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity);
        tooltip.SetActive(true);

        // Add the tooltip to the list of active tooltips
        activeTooltips.Add(tooltip);

        // Update tooltip position to be above the resourceFloating object
        tooltip.transform.position = resourceObject.transform.position + Vector3.up * 1f;

        // Calculate the direction from the resourceFloating object to the player
        Vector3 directionToPlayer = (transform.position - resourceObject.transform.position).normalized;

        // Calculate the rotation to face the player
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Apply the rotation, taking into account the current rotation of the tooltip
        tooltip.transform.rotation = rotationToPlayer * Quaternion.Euler(0, 180, 0); // Adjust the 180 degrees to correct the rotation

        setTextTo3DToolTip(tooltip, droppedResourceScript);
        // Optional: Update tooltip content (e.g., resource type, quantity, etc.)
    }

    void ClearTooltips()
    {
        // Destroy all active tooltips
        foreach (GameObject tooltip in activeTooltips)
        {
            Destroy(tooltip);
        }

        // Clear the list of active tooltips and reset the current tooltip count
        activeTooltips.Clear();
        currentTooltips = 0;
    }

    private void setTextTo3DToolTip(GameObject toolTip, DroppedResource resource){
         TextMeshPro textMeshPro = toolTip.GetComponent<TextMeshPro>();
         if(textMeshPro){
            textMeshPro.text = resource.getResource().getResourceName() + "\n Press E to collect";
         }
    }
}
