using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickResourceChecker : MonoBehaviour
{
    [SerializeField] private float maxDistance = 2f; // Adjust the maximum distance as needed
    [SerializeField] private ToolHandler toolHandler;

    // Update is called once per frame
    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the center of the screen
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 0.5f, 0.5f is the center of the screen
            RaycastHit hit;

            // Check if the ray hits any collider within the specified distance
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // Check if the hit object has the tag "Resource"
                if (hit.collider.CompareTag("Resource"))
                {
                    // Check if the distance is within the allowed range
                    if (hit.distance <= maxDistance)
                    {
                        ResourceHandler resourceHandler = hit.collider.GetComponent<ResourceHandler>();
                        if(resourceHandler){
                            resourceHandler.giveDamage(10f, toolHandler.getToolType());
                        }
                    
                    }
                }
            }
        }
    }
}
