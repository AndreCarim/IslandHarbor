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
    [SerializeField] private BlackSmithUiHandler blackSmithUIHandlerScript;

    void Start(){
        initialize();
    }

    private void SetupButtonListener()
    {
        Button button = GetComponent<Button>(); // Get the Button component attached to this GameObject
        if (button != null)
        {
            button.onClick.AddListener(ButtonClicked); // Add listener for button click event
        }
        else
        {
            Debug.LogWarning("No Button component found on the GameObject.", this);
        }
    }

    private void ButtonClicked()
    {
        blackSmithUIHandlerScript.buttonSelectedSetPrice(recipe);
    }

    private void initialize(){
        if(!recipe){//if there is no recipe to the button, just hide the button
            gameObject.SetActive(false);
            return;
        } 

        ResourceGenericHandler prize = recipe.getPrize();

        icon.sprite = prize.getIcon();
        nameText.text = prize.getName();
        powerText.text = prize.getDamage().ToString();

        SetupButtonListener();
    }
}
