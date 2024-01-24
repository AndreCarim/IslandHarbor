using System.Collections;
using UnityEngine;
using TMPro;

public class ResourceHandler : MonoBehaviour
{
    [SerializeField] private double startHealth;
    private double currentHealth;

    private Material defaultMaterial;
    private Material blinkMaterial;

    [SerializeField] private ResourceGenericHandler resourceDrop;

    [SerializeField] private int amountDropFrom = 1;
    [SerializeField] private int amountDropTo = 1;

    [Tooltip("Leave -1 to simply destroy the resource")]
    [SerializeField] private float secondsToRespawnFrom = -1;
    
    [Tooltip("Leave -1 to simply destroy the resource")]
    [SerializeField] private float secondsToRespawnTo = -1;
    

    [SerializeField] private ToolEnum.ToolType toolType;


    

    private Renderer resourceRenderer; // Reference to the resource renderer

    void Start()
    {
        currentHealth = startHealth;

        // Attempt to get the Renderer component from the same GameObject
        resourceRenderer = GetComponent<Renderer>();

        if (resourceRenderer == null)
        {
            Debug.LogError("ResourceHandler: Resource Renderer not found on the same GameObject!");
        }
        else
        {
            defaultMaterial = resourceRenderer.material;
        }
    }

    public void giveDamage(double damageAmount, ToolEnum.ToolType toolTypeUsed, GameObject player)
    {
        if (currentHealth > 0)
        {
            if (toolTypeUsed == toolType)
            {
                currentHealth -= damageAmount;
            }
            else
            {
                currentHealth -= (damageAmount / 2);
            }
            //Debug.Log(currentHealth);
            StartCoroutine(Effects());
            checkHealthStats(player);
        }
    }

    private void checkHealthStats(GameObject player)
    {
        if (currentHealth <= 0)
        {
            //resource died
            // Get a random number between amountDropFrom (inclusive) and amountDropTo (exclusive)
            int randomAmount = Random.Range(amountDropFrom, amountDropTo + 1);
            player.GetComponent<ResourceInventory>().AddResource(resourceDrop, randomAmount);
        }
    }

    IEnumerator Effects()
    {
        if (resourceRenderer != null)
        {
            // Store the original color and position
            Color originalColor = resourceRenderer.material.color;
            Vector3 originalPosition = transform.position;

            // Apply blink material with semi-transparent white color
            resourceRenderer.material = blinkMaterial;
            resourceRenderer.material.color = new Color(1f, .5f, .4f, 0f); // Adjust the alpha value for transparency

            // Shake parameters
            float shakeDuration = 0.15f; // Adjust the duration of the shaking
            float shakeMagnitude = 0.5f; // Adjust the magnitude of the shaking

            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                // Calculate the amount of shaking
                float x = originalPosition.x + Random.Range(-1f, 1f) * shakeMagnitude;
                float y = originalPosition.y + Random.Range(-1f, 1f) * shakeMagnitude;

                // Apply the shaking to the object's position
                transform.position = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Revert to the default material, original color, and position
            resourceRenderer.material = defaultMaterial;
            resourceRenderer.material.color = originalColor;
            transform.position = originalPosition;
        }
        else
        {
            Debug.LogError("ResourceHandler: Resource Renderer not found on the same GameObject!");
        }
    }

    

}
