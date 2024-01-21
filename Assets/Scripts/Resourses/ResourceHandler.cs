using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Responsible for dealing with all the resources items in the game
*/

public class ResourceHandler : MonoBehaviour
{


    [SerializeField] private double startHealth;
    [SerializeField] private double currentHealth;

    //random seconds to respanw, for example from 10 to 20 seconds
    [SerializeField] private int secondsToRespawnFrom;
    [SerializeField] private int secondsToRespawnTo;

    [SerializeField] private int amountDroppedFrom;
    [SerializeField] private int amountDroppedTo;

    [SerializeField] private ToolHandler.ToolType toolType;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startHealth;
    }


    public void giveDamage(double damageAmount, ToolHandler.ToolType toolTypeUsed){
        if(currentHealth > 0 ){
            if(toolTypeUsed == toolType){
                currentHealth -= damageAmount;
                Debug.Log(currentHealth);
            }else{
                currentHealth -= (damageAmount / 2);
                Debug.Log(currentHealth);
            }
            
            checkHealthStats();
           
        } 
    }

    private void checkHealthStats(){
        if(currentHealth <= 0){
           Debug.Log("Resource is dead");
            
        }
    }
}
