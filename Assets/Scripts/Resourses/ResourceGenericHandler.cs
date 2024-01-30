using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Resource", menuName = "New Resource")]
public class ResourceGenericHandler : ScriptableObject
{
    [SerializeField] private string resourceName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int id;
    [SerializeField] private GameObject dropGameObject;

    [SerializeField] private int maxAmountInInventory;
    [SerializeField] private string informationText;

    public int getId(){
        return id;
    }

    public string getResourceName(){
        return resourceName;
    }

    public Sprite getIcon(){
        return icon;
    }

    public int getMaxAmountInInventory(){
        return maxAmountInInventory;
    }

    public GameObject getDropGameObject(){
        return dropGameObject;
    }

    public string getInformationText(){
        return informationText;
    }
} 
