using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CenterUIHandler : MonoBehaviour
{
    [SerializeField] private Image centerDot;
    [SerializeField] private Slider cooldownSlider;


    void Start(){
        //if(!IsOwner){return;}
        cooldownSlider.gameObject.SetActive(false); // Disable cooldown slider at start
    }

    public void setCenterDotColor(Color color){
        //if(!IsOwner){return;}
        centerDot.color = color;

    }

    public void setCooldownSlider(float value){
       // if(!IsOwner){return;}
        cooldownSlider.value = value;
    }

    public void startCooldownSlider(){
        //if(!IsOwner){return;}
         // Enable the slider before starting the cooldown
        cooldownSlider.gameObject.SetActive(true);
        cooldownSlider.value = 1f;
    }

    public void endCooldownSlider(){
        //if(!IsOwner){return;}
         // Ensure the slider is set to 0 when the cooldown is complete
        cooldownSlider.value = 0f;

        // Disable the slider after the cooldown
        cooldownSlider.gameObject.SetActive(false);
    }
}
