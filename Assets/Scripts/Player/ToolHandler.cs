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
    private ResourceGenericHandler currentEquipment;
    private ToolUIHandler toolUIHandler; // calls functions from the tooluihandler inside the ui

    [SerializeField] private ResourceGenericHandler currentAxe;
    [SerializeField] private ResourceGenericHandler currentPickAxe;
    [SerializeField] private ResourceGenericHandler currentWeapon;




    public override void OnNetworkSpawn(){
        currentEquipment = currentAxe;
        
    }

    //this will handle the changing of tool pressing 1,2 and 3
    void Update()
    {

        if(!IsOwner) return;

        // Check if the player pressed the 1 key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(currentEquipment != currentAxe){
                currentEquipment = currentAxe;
                toolUIHandler.setIsOn(0);
            }
        }

        // Check if the player pressed the 2 key
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(currentEquipment != currentPickAxe){
                currentEquipment = currentPickAxe;
                toolUIHandler.setIsOn(1);
            }
            // Call a function or perform an action related to key 2
        }

        // Check if the player pressed the 3 key
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(currentEquipment != currentWeapon){
                currentEquipment = currentWeapon;
                toolUIHandler.setIsOn(2);
            }
            // Call a function or perform an action related to key 3
        }
    }

   

    public ResourceGenericHandler getCurrentEquipment(){
        return currentEquipment;
    }

    public ResourceGenericHandler getCurrentAxe(){
        return currentAxe;
    }

    public ResourceGenericHandler getCurrentPickAxe(){
        return currentPickAxe;
    }
    
    public ResourceGenericHandler getCurrentWeapon(){
        return currentWeapon;
    }

    public void setCurrentAxe(ResourceGenericHandler equipment){
        if(!IsOwner) return;

        if(equipment.getResourceType() == ResourceEnum.ResourceType.Axe){
            //player is trying to equip the right equipment
            Debug.Log("Axe Equipped");
        }
    }

    public void setCurrentPickAxe(ResourceGenericHandler equipment){
        if(!IsOwner) return;

        if(equipment.getResourceType() == ResourceEnum.ResourceType.PickAxe){
            //player is trying to equip the right equipment
            Debug.Log("PickAxe Equipped");
        }
    }

    public void setCurrentWeapon(ResourceGenericHandler equipment){
        if(!IsOwner) return;

        if(equipment.getResourceType() == ResourceEnum.ResourceType.Weapon){
            //player is trying to equip the right equipment
            currentWeapon = equipment;

            currentEquipment = currentWeapon;
            toolUIHandler.setIsOn(2);
        }
    }

    public void setOnStart( GameObject entirePlayerUIInstance){
        toolUIHandler = entirePlayerUIInstance.GetComponent<ToolUIHandler>();
        toolUIHandler.setIsOn(0);
    }
}
