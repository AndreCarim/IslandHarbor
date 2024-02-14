using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Resource", menuName = "New Resource")]
public class ResourceGenericHandler : ScriptableObject
{

    [SerializeField] private ResourceEnum.ResourceType resourceType;
    [SerializeField] private string Name;
    [SerializeField] private Sprite icon;
    [SerializeField] private int id;
    [SerializeField] private GameObject dropGameObject;
    [SerializeField] private int goldValue;

    [SerializeField] private int weight;
    [SerializeField] private string informationText;

    [Header("Only for EQUIPMENTS!")]
    
    [SerializeField] private double damage;
    private float debounceTime = 1;
  

    public int getId(){
        return id;
    }

    public string getName(){
        return Name;
    }

    public Sprite getIcon(){
        return icon;
    }

    public GameObject getDropGameObject(){
        return dropGameObject;
    }

    public string getInformationText(){
        return informationText;
    }

    public int getWeight(){
        return weight;
    }

    public double getDamage(){
        return damage;
    }

    public float getDebounceTime(){
        return debounceTime;
    }

   public ResourceEnum.ResourceType getResourceType(){
        return resourceType;
    }

    public int getGoldValue(){
        return goldValue;
    }
} 
