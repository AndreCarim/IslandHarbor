using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ToolUIHandler : NetworkBehaviour
{
    [SerializeField] private Image pickAxeIsOn;
    [SerializeField] private Image axeIsOn;
    [SerializeField] private Image weaponIsOn;

    private ResourceGenericHandler currentAxe;
    private ResourceGenericHandler currentPickAxe;
    private ResourceGenericHandler currentWeapon;

    [SerializeField] private Image axeIcon;
    [SerializeField] private Image pickAxeIcon;
    [SerializeField] private Image weaponIcon;

    public void setIsOn(int WhichUI)
    {
        if(!IsOwner){return;}

        pickAxeIsOn.color = new Color(pickAxeIsOn.color.r, pickAxeIsOn.color.g, pickAxeIsOn.color.b, 0f);
        axeIsOn.color = new Color(axeIsOn.color.r, axeIsOn.color.g, axeIsOn.color.b, 0f);
        weaponIsOn.color = new Color(weaponIsOn.color.r, weaponIsOn.color.g, weaponIsOn.color.b, 0f);

        if(WhichUI == 0){
            //axe equipped
            axeIsOn.color = new Color(axeIsOn.color.r, axeIsOn.color.g, axeIsOn.color.b, 1f);
        }else if(WhichUI == 1){
            //pickaxe equipped
            pickAxeIsOn.color = new Color(pickAxeIsOn.color.r, pickAxeIsOn.color.g, pickAxeIsOn.color.b, 1f);
        }else if(WhichUI == 2){
            //weapon equipped
            weaponIsOn.color = new Color(weaponIsOn.color.r, weaponIsOn.color.g, weaponIsOn.color.b, 1f);
        }       
    }

    public void equipEquipment(ResourceGenericHandler equipment){
        if(equipment.getResourceType() == ResourceEnum.ResourceType.Axe){
            currentAxe = equipment;
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.PickAxe){
            currentPickAxe = equipment;
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.Weapon){
            currentWeapon = equipment;
        }

        setUiImages();   
    }


    public void removeEquippedEquipmentByDropOrUnequip(ResourceGenericHandler equipment){
        if(equipment.getResourceType() == ResourceEnum.ResourceType.Axe){
            currentAxe = null;
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.PickAxe){
            currentPickAxe = null;
        }else if(equipment.getResourceType() == ResourceEnum.ResourceType.Weapon){
            currentWeapon = null;    
        }
        setUiImages();
    }

    private void setUiImages(){
        if(currentAxe){
            axeIcon.sprite = currentAxe.getIcon();
        }else{
            axeIcon.sprite = null;
        }

        if(currentPickAxe){
            pickAxeIcon.sprite = currentPickAxe.getIcon();
        }else{
            pickAxeIcon.sprite = null;
        }

        if(currentWeapon){
            weaponIcon.sprite = currentWeapon.getIcon();
        }else{
            weaponIcon.sprite = null;
        }
    }

}
