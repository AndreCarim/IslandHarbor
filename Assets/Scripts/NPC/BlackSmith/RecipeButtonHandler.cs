using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class RecipeButtonHandler : MonoBehaviour
{
    [Header("Recipe for this button")]
    [SerializeField] private RecipeGeneric recipe;

    [Header("------------------------")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI powerText;

    void Start(){
        initialize();
    }

    private void initialize(){
        if(!recipe) return;

        ResourceGenericHandler prize = recipe.getPrize();

        icon.sprite = prize.getIcon();
        nameText.text = prize.getName();
        powerText.text = prize.getDamage().ToString();
    }
}
