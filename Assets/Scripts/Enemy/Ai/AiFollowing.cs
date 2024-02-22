using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class AiFollowing : NetworkBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform player;

    public override void OnNetworkSpawn(){
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player && agent && IsServer){
            agent.destination = player.position;
        }
        
    }
}
