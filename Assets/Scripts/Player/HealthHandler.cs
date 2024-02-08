using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthHandler : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<double> maxHealth = new NetworkVariable<double>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<double> currentHealth = new NetworkVariable<double>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);




    public override void OnNetworkSpawn(){
        if(!IsOwner) return;

        currentHealth.Value = maxHealth.Value;
    }

    public void gainCurrentHealth(double value){
        if(!IsOwner) return;

        if(currentHealth.Value + value <= maxHealth.Value){
            //player can gain all value life
            currentHealth.Value += value;
        }else{
            //value + currentHealth is greater than maxHealth
            //so just give current health to be equal to the max health. 
            currentHealth.Value = maxHealth.Value;
        }

    }

    public void damageCurrentHealth(double value){
        if(!IsOwner) return;

        currentHealth.Value -= value;
        checkHealthStats();
    }

    private void checkHealthStats(){
        if(currentHealth.Value <= 0){
            //player died
            Debug.Log("Player died");
        }else{
            //player is still alive, we can do some red screen if the hp is low
            //also, we can add heart beat sound if the health is low.
            if(currentHealth.Value < 20){
                Debug.Log("Danger! Health is lower than 20");
            }
        }
    }


    public double getMaxHealth(){
        return maxHealth.Value;
    } 

    public double getCurrentHealth(){
        return currentHealth.Value;
    }
}
