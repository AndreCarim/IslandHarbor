using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class BlackSmithUiHandler : NetworkBehaviour
{
    [SerializeField] private GameObject axeTab;
    [SerializeField] private GameObject pickaxeTab;
    [SerializeField] private GameObject weaponTab;


    public void openAxeTab(){
        if(!IsOwner)return;

        resetTabs();
        axeTab.SetActive(true);
    }

    public void openPickaxeTab(){
        if(!IsOwner)return;

        resetTabs();
        pickaxeTab.SetActive(true);
    }

    public void openWeaponTab(){
        if(!IsOwner)return;

        resetTabs();
        weaponTab.SetActive(true);
    }

    private void resetTabs(){
        if(!IsOwner)return;

        axeTab.SetActive(false);
        pickaxeTab.SetActive(false);
        weaponTab.SetActive(false);
    }
}
