using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "New Recipe")]
public class RecipeGeneric : ScriptableObject
{
    [Header("The list of ingredients the player need to have")]
    [SerializeField] private List<IngredientGeneric> ingredientList;

    [Header("The prize that the player will get once he fulfills the ingredientList")]
    [SerializeField] private ResourceGenericHandler prize;

    [SerializeField] private int prizeAmount;

    public ResourceGenericHandler getPrize(){
        return prize;
    }

    public List<IngredientGeneric> getIngredientList(){
        return ingredientList;
    }

    public int getPrizeAmount(){
        return prizeAmount;
    }

}
