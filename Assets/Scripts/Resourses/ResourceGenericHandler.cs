using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Resource", menuName = "New Resource")]
public class ResourceGenericHandler : ScriptableObject
{
    [SerializeField] private string resourceName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int id;

    public int getId(){
        return id;
    }

    public string getResourceName(){
        return resourceName;
    }

    public Sprite getIcon(){
        return icon;
    }
} 
