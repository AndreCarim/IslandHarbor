using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using Unity.Netcode;

public class BlackSmithUiHandler : MonoBehaviour
{
    [SerializeField] private GameObject axeTab;
    [SerializeField] private GameObject pickaxeTab;
    [SerializeField] private GameObject weaponTab;

    [SerializeField] private GameObject player;

    [Header("-------------------")]
    [SerializeField] private GameObject priceTab;
    [SerializeField] private GameObject genericSlot; 
    [SerializeField] private GameObject slotContent;
    [SerializeField] private GameObject craftButton;
    [SerializeField] private GameObject blackSmithUI;

    private Color redColor = new Color(0.9529f, 0.6f, 0.6f); // Assigning a light red color
    private Color greenColor = new Color(0.6039f, 0.9804f, 0.6745f); // Assigning a color close to #9FFAAC

    private RecipeGeneric currentRecipe;
    private List<GameObject> slots = new List<GameObject>();

    //being called by the buttons inside the viewport/content of the mainframe
    public void buttonSelectedSetPrice(RecipeGeneric recipe){
        reset();

        ResourceInventory resourceInventory = player.GetComponent<ResourceInventory>();

        currentRecipe = recipe;

        bool haveAllItems = true;//it will keep true if the player has all
        
        foreach(IngredientGeneric Ingredient in recipe.getIngredientList()){
            // Instantiate the genericSlot GameObject
            GameObject newSlot = Instantiate(genericSlot, slotContent.GetComponent<Transform>());

            // Find the child GameObjects
            Image icon = newSlot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI youNeedAmountText = newSlot.transform.Find("YouNeedAmountText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI youHaveAmountText = newSlot.transform.Find("YouHaveAmountText").GetComponent<TextMeshProUGUI>();
            
            int youNeed = Ingredient.getAmount();
            int youHave = resourceInventory.checkItemAmount(Ingredient.getResource().getId());

            icon.sprite = Ingredient.getResource().getIcon();
            youNeedAmountText.text = youNeed.ToString();
            youHaveAmountText.text = youHave.ToString();

            newSlot.SetActive(true);

            if(youHave >= youNeed){
                //player has enough of this material
                newSlot.GetComponent<Image>().color = greenColor;
            }else{
                //player does not have enought of this material
                newSlot.GetComponent<Image>().color = redColor;
                haveAllItems = false;
            }
            

            // Add the newSlot to the slots list
            slots.Add(newSlot);
        }

        if(haveAllItems){
            craftButton.SetActive(true);
        }else{
            craftButton.SetActive(false);
        }

        priceTab.SetActive(true);
    }

    public void craftButtonClicked(){
        collectTheIngredientsFromPlayer();
    }

    private void collectTheIngredientsFromPlayer(){
        ResourceInventory resourceInventory = player.GetComponent<ResourceInventory>();

        if(!currentRecipe || !resourceInventory) return;

        //check if the player has everything
        bool haveAllItems = true;

        foreach(IngredientGeneric Ingredient in currentRecipe.getIngredientList()){
            int youNeed = Ingredient.getAmount();
            int youHave = resourceInventory.checkItemAmount(Ingredient.getResource().getId());

            if(youHave < youNeed){
                haveAllItems = false;
                break;
            }
        }

        if(haveAllItems){
            //player has all the items, now subtract the items from backpack
            foreach(IngredientGeneric Ingredient in currentRecipe.getIngredientList()){
                resourceInventory.RemoveResource(Ingredient.getResource(), Ingredient.getAmount());
            }

            givePrizeToPlayer();
        }else{
            Debug.Log("Player does not have all items");
        }
    }

    private void givePrizeToPlayer(){
         ResourceInventory resourceInventory = player.GetComponent<ResourceInventory>();

        if(!currentRecipe || !resourceInventory) return;

        resourceInventory.AddResource(currentRecipe.getPrize(), currentRecipe.getPrizeAmount());

        buttonSelectedSetPrice(currentRecipe);
    }

    public void openUI(){
        reset();
        resetTabs();
        priceTab.SetActive(false);
        openAxeTab();

        blackSmithUI.SetActive(true);
    }

    private void reset(){
        //clean all the slots
        currentRecipe = null;
        craftButton.SetActive(false);

        if(slots.Count == 0) return;

        foreach (GameObject obj in slots)
        {
            Destroy(obj);
        }
        slots.Clear(); 

        
    }


    public void openAxeTab(){
        //if(!IsOwner)return;

        resetTabs();
        axeTab.SetActive(true);
    }

    public void openPickaxeTab(){
        //if(!IsOwner)return;

        resetTabs();
        pickaxeTab.SetActive(true);
    }

    public void openWeaponTab(){
       // if(!IsOwner)return;

        resetTabs();
        weaponTab.SetActive(true);
    }

    private void resetTabs(){
       //if(!IsOwner)return;

        axeTab.SetActive(false);
        pickaxeTab.SetActive(false);
        weaponTab.SetActive(false);

        priceTab.SetActive(false);

        reset();
    }
}
