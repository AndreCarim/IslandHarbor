using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueGeneric 
{
    public string npcName;

    [TextArea(3, 10)]
    public string[] sentences;
    [Space]

    [Header("If the npc gives a prize at the end of the senteces")]
    public bool npcGivesPrize;
    public int prizeAmount;
    public ResourceGenericHandler[] prize;

    [Header("If the npc requires a resource in return")]
    public ResourceGenericHandler cost; 
    public int costAmount;

    [Header("Check if the npc will vanish after giving prize")]
    public bool willVanishAfterPrize;
    [Header("check if the npc will vanish after talking to the player")]
    public bool willVanishAfterTalking;

}
