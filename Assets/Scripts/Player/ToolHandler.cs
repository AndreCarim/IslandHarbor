using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

//this script is responsible for picking up which tool you have
//it also sets which kind of tools there are.
public class ToolHandler : NetworkBehaviour
{
    private ToolGenericHandler currentTool;
    private ToolUIHandler toolUIHandler; // calls functions from the tooluihandler inside the ui

    [SerializeField] private ToolGenericHandler currentAxe;
    [SerializeField] private ToolGenericHandler currentPickAxe;
    [SerializeField] private ToolGenericHandler currentWeapon;


    public override void OnNetworkSpawn(){
        currentTool = currentAxe;
        
    }

    //this will handle the changing of tool pressing 1,2 and 3
    void Update()
    {

        if(!IsOwner) return;

        // Check if the player pressed the 1 key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(currentTool != currentAxe){
                currentTool = currentAxe;
                toolUIHandler.setIsOn(0);
            }
        }

        // Check if the player pressed the 2 key
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(currentTool != currentPickAxe){
                currentTool = currentPickAxe;
                toolUIHandler.setIsOn(1);
            }
            // Call a function or perform an action related to key 2
        }

        // Check if the player pressed the 3 key
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(currentTool != currentWeapon){
                currentTool = currentWeapon;
                toolUIHandler.setIsOn(2);
            }
            // Call a function or perform an action related to key 3
        }
    }

    public ToolGenericHandler getCurrentTool(){
        return currentTool;
    }

    public ToolGenericHandler getCurrentAxe(){
        return currentAxe;
    }

    public ToolGenericHandler getCurrentPickAxe(){
        return currentAxe;
    }
    
    public ToolGenericHandler getCurrentWeapon(){
        return currentAxe;
    }

    public void setOnStart( GameObject entirePlayerUIInstance){
        toolUIHandler = entirePlayerUIInstance.GetComponent<ToolUIHandler>();
        toolUIHandler.setIsOn(0);
    }
}
