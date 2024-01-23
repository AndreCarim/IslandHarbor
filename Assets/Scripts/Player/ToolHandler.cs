using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


//this script is responsible for picking up which tool you have
//it also sets which kind of tools there are.
public class ToolHandler : MonoBehaviour
{
    public enum ToolType{
        Axe,
        PickAxe,
        Weapon
    }

    private ToolGenericHandler currentTool;

    [SerializeField] private Image pickAxeIsOn;
    [SerializeField] private Image axeIsOn;
    [SerializeField] private Image weaponIsOn;

    [SerializeField] private ToolGenericHandler axe;
    [SerializeField] private ToolGenericHandler pickAxe;
    [SerializeField] private ToolGenericHandler weapon;



    void Start(){
        currentTool = axe;
        setIsOn(axeIsOn);
    }

    //this will handle the changing of tool pressing 1,2 and 3
    void Update()
    {
        // Check if the player pressed the 1 key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(currentTool != axe){
                currentTool = axe;
                setIsOn(axeIsOn);
            }
        }

        // Check if the player pressed the 2 key
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(currentTool != pickAxe){
                currentTool = pickAxe;
                setIsOn(pickAxeIsOn);
            }
            // Call a function or perform an action related to key 2
        }

        // Check if the player pressed the 3 key
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(currentTool != weapon){
                currentTool = weapon;
                setIsOn(weaponIsOn);
            }
            // Call a function or perform an action related to key 3
        }
    }

    public ToolGenericHandler getCurrentTool(){
        return currentTool;
    }

    private void setIsOn(Image isOnUI)
    {
        pickAxeIsOn.color = new Color(pickAxeIsOn.color.r, pickAxeIsOn.color.g, pickAxeIsOn.color.b, 0f);
        axeIsOn.color = new Color(axeIsOn.color.r, axeIsOn.color.g, axeIsOn.color.b, 0f);
        weaponIsOn.color = new Color(weaponIsOn.color.r, weaponIsOn.color.g, weaponIsOn.color.b, 0f);

        isOnUI.color = new Color(isOnUI.color.r, isOnUI.color.g, isOnUI.color.b, 1f);
    }
}
