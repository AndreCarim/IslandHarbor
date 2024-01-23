using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Import the Unity UI namespace

public class ClickResourceChecker : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private ToolHandler toolHandler;
    [SerializeField] private Slider cooldownSlider; // Reference to the UI Slider

    private bool canClick = true;

    void Start()
    {
        cooldownSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check if the player is holding down the left mouse button
        if (Input.GetMouseButton(0) && toolHandler.getCurrentTool() != null)
        {
            if (canClick)
            {
                StartCoroutine(ClickCooldown(toolHandler.getCurrentTool()));

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
                            if (resourceHandler)
                            {
                                ToolGenericHandler currentTool = toolHandler.getCurrentTool();
                                resourceHandler.giveDamage(currentTool.getDamage(), currentTool.getToolType());
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator ClickCooldown(ToolGenericHandler currentTool)
    {
        canClick = false;
        float cooldownTime = currentTool.getDebounceTime();

        // Enable the slider before starting the cooldown
        cooldownSlider.gameObject.SetActive(true);
        cooldownSlider.value = 1f;

        while (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
            cooldownSlider.value = cooldownTime / currentTool.getDebounceTime();
            yield return null;
        }

        // Ensure the slider is set to 0 when the cooldown is complete
        cooldownSlider.value = 0f;

        // Disable the slider after the cooldown
        cooldownSlider.gameObject.SetActive(false);

        // Reset the canClick flag after the cooldown
        canClick = true;
    }
}
