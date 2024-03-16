using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueUIhandler : MonoBehaviour
{
    [SerializeField] private Transform dialogObject;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private Button nextButton;

    [SerializeField] private NPCUIHandler npcuiHandler;
    [SerializeField] private Animator animator;
    [SerializeField] private ResourceInventory inventoryScript;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject nextButtonObject;

    private Queue<string> sentences;

    private DialogueGeneric dialogue;
    private GameObject npcGameObject;

    private void Start() {
        sentences = new Queue<string>();

        nextButton.onClick.AddListener(displayNextSentence);
        yesButton.onClick.AddListener(yesButtonpressed);
        noButton.onClick.AddListener(noButtonPressed);
    }

    public void startDialogue(DialogueGeneric dialogue, GameObject npcGameObject){
        animator.SetBool("IsOpen", true);

        this.dialogue = dialogue;
        this.npcGameObject = npcGameObject;

        npcName.text = dialogue.npcName;

        resetButtons();

        foreach(string sentence in dialogue.sentences){
            sentences.Enqueue(sentence);
        }

        displayNextSentence();
    }

    private void displayNextSentence(){
        if(sentences.Count == 0){
            //reached the end of the queue
            if(dialogue != null && dialogue.npcGivesPrize == true){
                //npc will give some prize
                checkNPCGivePrizes();
                return;
            }

            endDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        content.text = sentence;
    }

    private void endDialogue(){
        //check if there is prizes
        npcuiHandler.openOrCloseDialogue();
        animator.SetBool("IsOpen", false);

        if(npcGameObject != null && dialogue != null && dialogue.willVanishAfterTalking == true){
            //npc will be destroyed
            Destroy(npcGameObject);
        }

        dialogue = null;
        npcGameObject = null;
    }

    private void checkNPCGivePrizes(){
        if(dialogue == null)return;

        if(dialogue.npcGivesPrize == true){
            //this npc gives prizes
            if(dialogue.cost != null){
                //there is a cost to this
                int youHave = inventoryScript.checkItemAmount(dialogue.cost.getId());

                if(dialogue.prize.Length == 1){
                    //only do that if the content of prize is one
                    content.text = "You will pay: " + dialogue.costAmount.ToString() + " of " + dialogue.cost.getName() +
                     ". You have: " + youHave.ToString() +" You will get: " + dialogue.prizeAmount.ToString() + " " + dialogue.prize[0].getName() + 
                     ". Are you sure you want to do that?";
                }
                
                if(youHave >= dialogue.costAmount){
                    //player has the cost
                    yesButton.gameObject.SetActive(true);
                    noButton.gameObject.SetActive(true);
                    nextButtonObject.gameObject.SetActive(false);
                }else{
                    //player dosent have
                    yesButton.gameObject.SetActive(false);
                    noButton.gameObject.SetActive(true);
                    nextButtonObject.gameObject.SetActive(false);
                }
            }else{
                //the npc will give the prize for free
                givePrizes();
                endDialogue();
            }
        }
    }

    private void resetButtons(){
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButtonObject.gameObject.SetActive(true);
    }

    private void noButtonPressed(){
        endDialogue();
    }

    private void yesButtonpressed(){
        removeResource();
        givePrizes();

        

        endDialogue();
    }

    private void givePrizes(){
        if(dialogue == null)return;
        foreach(ResourceGenericHandler resourcePrize in dialogue.prize){
            inventoryScript.AddResource(resourcePrize, dialogue.prizeAmount);
        }

        if(dialogue.willVanishAfterPrize == true){
            Destroy(npcGameObject);
        }
        
    }

    private void removeResource(){
        if(dialogue != null){
            inventoryScript.RemoveResource(dialogue.cost, dialogue.costAmount);
        }
    }
}
