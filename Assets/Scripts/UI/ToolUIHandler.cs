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
}
