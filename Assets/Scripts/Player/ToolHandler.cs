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

    [SerializeField] private GameObject toolHolder;

    private GameObject currentTool3D;


    private int currentWeightEquipped;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    //-------------------------------------//
    //            ANIMATIONS               //
    //-------------------------------------//

    

    [SerializeField]private Animator toolAnimator;

    private string currentAnimationState;



    public override void OnNetworkSpawn(){
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        onFoot.ToolOne.started += ctx => pressedOneButton();
        onFoot.ToolTwo.started += ctx => pressedTwoButton();
        onFoot.ToolThree.started += ctx => pressedThreeButton();

        currentEquipment = currentAxe;
        change3DEquipment();

        onFoot.Enable();
    }

    private void pressedOneButton(){
        if(!IsOwner) return;

        if(currentEquipment != currentAxe){
            currentEquipment = currentAxe;
            toolUIHandler.setIsOn(0);
        }
        change3DEquipment();
    }

    private void pressedTwoButton(){
        if(!IsOwner) return;

        if(currentEquipment != currentPickAxe){
            currentEquipment = currentPickAxe;
            toolUIHandler.setIsOn(1);
        }
        // Call a function or perform an action related to key 2
        change3DEquipment();

    }

    private void pressedThreeButton(){
        if(!IsOwner) return;

        if(currentEquipment != currentWeapon){
            currentEquipment = currentWeapon;
            toolUIHandler.setIsOn(2);
        }
        // Call a function or perform an action related to key 3
        change3DEquipment();
    }

    private void change3DEquipment()
    {
        if (!currentEquipment)
        {
                // Destroy the current tool if it exists
            if (currentTool3D != null)
            {
                Destroy(currentTool3D);
            }
            return;
        }

        // Destroy the current tool if it exists
        if (currentTool3D != null)
        {
            Destroy(currentTool3D);
        }

        // Instantiate the new tool with the toolHolder as its parent
        currentTool3D = Instantiate(currentEquipment.getDropGameObject(), toolHolder.transform);

        // Optionally, you can set the position and rotation of the new tool relative to the toolHolder
        currentTool3D.transform.localPosition = Vector3.zero;
        currentTool3D.transform.localRotation = Quaternion.identity;

        // Check if the new instantiated object has a Rigidbody and Collider
        Rigidbody rb = currentTool3D.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb); // Destroy the Rigidbody if it exists
        }

        Collider collider = currentTool3D.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider); // Destroy the Collider if it exists
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

    public int getCurrentWeightEquipped(){
        return currentWeightEquipped;
    }


    public void equipEquippment(ResourceGenericHandler equipment){
        if(!IsOwner) return;

        if(equipment.getResourceType() == ResourceEnum.ResourceType.Axe){
            //player is trying to equip the right axe
            if(currentAxe){
                removeEquippedEquipmentByDropOrUnequip(equipment);
            }
            
            currentAxe = equipment;

            currentEquipment = currentAxe;
            currentWeightEquipped += currentAxe.getWeight();
            toolUIHandler.equipEquipment(equipment);
            toolUIHandler.setIsOn(0);

            
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.PickAxe){
            //player is trying to equip the right pickaxe

            if(currentPickAxe){
                removeEquippedEquipmentByDropOrUnequip(equipment);
            }
            
            currentPickAxe = equipment;

            currentEquipment = currentPickAxe;
            currentWeightEquipped += currentPickAxe.getWeight();
            toolUIHandler.equipEquipment(equipment);
            toolUIHandler.setIsOn(1);
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.Weapon){
            if(currentWeapon){
                removeEquippedEquipmentByDropOrUnequip(equipment);
            }
            
            currentWeapon = equipment;

            currentEquipment = currentWeapon;
            currentWeightEquipped += currentWeapon.getWeight();
            toolUIHandler.equipEquipment(equipment);
            toolUIHandler.setIsOn(2);


        }
        change3DEquipment();
    }

    public void removeEquippedEquipmentByDropOrUnequip(ResourceGenericHandler equipment){
        if(!IsOwner) return;

        if(equipment.getResourceType() == ResourceEnum.ResourceType.Axe){
            //player is trying to remove the right axe
            if(currentAxe){
                currentWeightEquipped -= currentAxe.getWeight();
            }
            currentAxe = null;

            currentEquipment = currentAxe;
            toolUIHandler.removeEquippedEquipmentByDropOrUnequip(equipment);
            toolUIHandler.setIsOn(0);
            
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.PickAxe){
            //player is trying to remove the right pickaxe
            if(currentPickAxe){
                currentWeightEquipped -= currentPickAxe.getWeight();
            }
            currentPickAxe = null;

            currentEquipment = currentPickAxe;
            toolUIHandler.removeEquippedEquipmentByDropOrUnequip(equipment);
            toolUIHandler.setIsOn(1);
            
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.Weapon){
            //player is trying to remove the right weapon
            if(currentWeapon){
                currentWeightEquipped -= currentWeapon.getWeight();
            }
            currentWeapon = null;

            currentEquipment = currentWeapon;
            toolUIHandler.removeEquippedEquipmentByDropOrUnequip(equipment);
            toolUIHandler.setIsOn(2);

        }
        change3DEquipment();
    }

   


    //idle, walking and running are being called by the movement script
    //hit is being called by the raycasting script
    //changing tool is being called by self script (this)
    public void changeAnimationState(string newState){
        //stop the same animation from interrupting itself
        if (currentAnimationState == newState || !IsOwner) {return;}

        //Play the animation
        currentAnimationState = newState;
        
        toolAnimator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    public void setOnStart( GameObject entirePlayerUIInstance){
        toolUIHandler = entirePlayerUIInstance.GetComponent<ToolUIHandler>();
        toolUIHandler.setIsOn(0);
    }

    

     private void OnDisable() {
        if(!IsOwner) return;
        onFoot.Disable();
    }
}
