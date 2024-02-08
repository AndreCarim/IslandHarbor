using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputMovement : NetworkBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    private bool canWalk = true;

    public override void OnNetworkSpawn(){
        if(!IsOwner) return;

        

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        
        onFoot.Jump.performed += ctx => Jump();
        controller = GetComponent<CharacterController>();

        onFoot.Enable();
    }


    void FixedUpdate(){
        if(!IsOwner) return;
        isGrounded = controller.isGrounded;
        ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

 

    
    public void ProcessMove(Vector2 input){

        if(isGrounded && playerVelocity.y < 0){
            playerVelocity.y = -2;
        }

        if(canWalk == true){
            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = input.x;
            moveDirection.z = input.y;

            controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        
        }

        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump(){
        if(isGrounded && canWalk){
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity); 
        }
    }

    public void setCanWalk(bool value){
        canWalk = value;
    }
}
