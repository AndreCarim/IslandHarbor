using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "New Ingredient")]
public class IngredientGeneric : ScriptableObject
{
    [Header("The resource The Player need to have")]
    [SerializeField] private ResourceGenericHandler resource;

    [Header("The amount of this resource the player need")]
    [SerializeField] private int amount;

    
    public ResourceGenericHandler getResource(){
        return resource;
    }

    public int getAmount(){
        return amount;
    }
}
