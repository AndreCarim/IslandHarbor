using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedResource : MonoBehaviour
{
    private float minDestroyTime = 60f; // Minimum time before auto destruction
    private float maxDestroyTime = 120f; // Maximum time before auto destruction

    private int amount;
    private ResourceGenericHandler resource;

    void Start()
    {   // Generate a random time interval for destruction
        float destroyTime = Random.Range(minDestroyTime, maxDestroyTime);
        
        // Destroy the GameObject after the random time interval
        Destroy(gameObject, destroyTime);
    }

    public void setResource(int amount, ResourceGenericHandler resource)
    {
        this.amount = amount;
        this.resource = resource;
    }

    public ResourceGenericHandler getResource(){
        return resource;
    }

    public int getAmount(){
        return amount;
    }
}
