using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class ResourceHPBar : MonoBehaviour
{
    [SerializeField] private GameObject hPTopScreen; // Health bar UI
    [SerializeField] private TextMeshProUGUI hpTopScreenText; // Health bar text
    [SerializeField] private Slider hpTopScreenSlider; // Health bar slider

    // Previous health value to track changes
    private float previousHealthValue = float.MinValue;

    public void setActiveHPBar(bool value){
        //if(!IsOwner){return;}
        hPTopScreen.SetActive(value);
    }

    // Update health bar of breakable objects
    public void setHpBar(ResourceHandler resourceHandler)
    {
        //if(!IsOwner){return;}
        // Activate health bar if not already active
        if (!hPTopScreen.activeSelf)
        {
            hPTopScreen.SetActive(true);
            hpTopScreenText.text = resourceHandler.getName(); // Update health bar text
        }

        // Only update if the health value has changed or the UI becomes visible
        if (hPTopScreen.activeSelf && (float)resourceHandler.getCurrentHealth() != previousHealthValue)
        {
            // Convert double values to float for Slider component
            float maxHealth = (float)resourceHandler.getStartHealth();
            float currentHealth = (float)resourceHandler.getCurrentHealth();

            // Set the values to the Slider
            hpTopScreenSlider.maxValue = maxHealth;
            hpTopScreenSlider.value = currentHealth;

            // Update the previous health value
            previousHealthValue = currentHealth;
        }
    }

    public void startCoroutines(){
        //if(!IsOwner){return;}
        // Animate hpTextAnimation
        StartCoroutine(AnimateHPText());
        // Shake the slider
        StartCoroutine(ShakeSlider(hpTopScreenSlider));
        // Briefly change the color to red and then back to white
        StartCoroutine(ChangeSliderColor(hpTopScreenSlider));
    }

    // Coroutine to shake the slider
    private IEnumerator ShakeSlider(Slider slider)
    {
        Vector3 originalPosition = slider.transform.position;
        float shakeAmount = 10f; // Increase shake amount
        float shakeDuration = 0.3f; // Increase shake duration
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            slider.transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset slider position
        slider.transform.position = originalPosition;
    }

    public void resetPreviousHealthValue(){
       // if(!IsOwner){return;}
        previousHealthValue = 0f;
    }

    // Coroutine to change the color of the slider briefly
    private IEnumerator ChangeSliderColor(Slider slider)
    {
        Color originalColor = slider.fillRect.GetComponent<Image>().color; // Store current color
        
        // Define a softer red color with reduced intensity
        Color softerRedColor = new Color(1f, 0.5f, 0.5f); // Adjust the values as needed
        
        slider.fillRect.GetComponent<Image>().color = softerRedColor; // Change to softer red color

        yield return new WaitForSeconds(0.1f); // Adjust duration as needed

        slider.fillRect.GetComponent<Image>().color = originalColor; // Restore original color
    }

    // Coroutine to animate hpTextAnimation
    private IEnumerator AnimateHPText()
    {
        // Add your animation logic for hpTextAnimation here
        // For example, you can use LeanTween or other animation libraries
        // to make the text animate from the center of the slider
        yield return null;
    }
}
