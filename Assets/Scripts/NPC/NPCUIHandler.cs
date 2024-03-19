using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject blackSmithUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private BlackSmithUiHandler blackSmithUIHandlerScript;
    [SerializeField] private DialogueUIhandler dialogueUIHandler;

 
    [SerializeField] private GameObject player;

    public void openOrCloseBlackSmithUI()
    {
        
        if (blackSmithUI.activeSelf)
        {
            // close
            blackSmithUI.SetActive(false);
            player.GetComponent<RayCastingHandler>().setIsAnyUIOpen(false);

            setConfig(true);
        }
        // If the blacksmith UI is not active (closed), open it.
        else
        {
            //open
            blackSmithUI.SetActive(true);
            player.GetComponent<RayCastingHandler>().setIsAnyUIOpen(true);

            blackSmithUIHandlerScript.openUI();
            setConfig(false);

        }
    }

    public void openOrCloseDialogue(GameObject npcGameObject = null, DialogueGeneric dialogue = null){
         if (dialogueUI.activeSelf)
        {
            // close
            dialogueUI.SetActive(false);
            player.GetComponent<RayCastingHandler>().setIsAnyUIOpen(false);

            setConfig(true);
        }
        // If the blacksmith UI is not active (closed), open it.
        else
        {
            //open
            dialogueUI.SetActive(true);
            player.GetComponent<RayCastingHandler>().setIsAnyUIOpen(true);

            setConfig(false);
            if(dialogue != null && npcGameObject != null){
                dialogueUIHandler.startDialogue(dialogue, npcGameObject);
            }

        }
    }

    private void setConfig(bool value){
        player.GetComponent<InputMovement>().setCanWalk(value); // player cant walk
        player.GetComponent<CameraInput>().setIsFreeToLook(value);
        player.GetComponent<ResourceInventory>().setCanOpenInventory(value);
        player.GetComponent<ResourceInventory>().setCanOpenInventory(value);
    }

    
}
