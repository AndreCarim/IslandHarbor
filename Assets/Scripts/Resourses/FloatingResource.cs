using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingResource : MonoBehaviour
{
    private float minFloatSpeed = .5f; // Speed of the floating motion
    private float maxFloatSpeed = 1.5f;
    private float floatSpeed;

    private float minFloatHeight = 0.5f; // Height of the float motion
    private float maxFloatHeight = 0.8f;
    private float floatHeight;

    private float minDestroyTime = 60f; // Minimum time before auto destruction
    private float maxDestroyTime = 120f; // Maximum time before auto destruction

    private float groundLevel;
    private Vector3 initialPosition;

    private int amount;
    private ResourceGenericHandler resource;

    void Start()
    {
        floatHeight = Random.Range(minFloatHeight, maxFloatHeight);
        floatSpeed = Random.Range(minFloatSpeed, maxFloatSpeed);
        // Store the initial position of the item
        initialPosition = transform.position;

        // Find the ground level using a raycast
        FindGroundLevel();

        // Generate a random time interval for destruction
        float destroyTime = Random.Range(minDestroyTime, maxDestroyTime);
        
        // Destroy the GameObject after the random time interval
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        if(floatHeight > 0 && floatSpeed > 0){
            // Calculate the new Y position based on a sinusoidal function and the ground level
            float newY = groundLevel + Mathf.Abs(Mathf.Sin(Time.time * floatSpeed) * floatHeight);

            // Update the item's position
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
      
    }

    void FindGroundLevel()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            groundLevel = hit.point.y + floatHeight; // Adjust ground level to start above the ground
        }
        else
        {
            // If no ground is found, set a default ground level
            groundLevel = initialPosition.y + floatHeight; // Adjust default ground level
        }
    }

    public void setResource(int amount, ResourceGenericHandler resource)
    {
        this.amount = amount;
        this.resource = resource;
    }
}