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

    private bool isAttacking = false; // this is for the animations

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
        setAnimation(onFoot.Movement.ReadValue<Vector2>());

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

    private void setAnimation(Vector2 input){
        if(!isAttacking){
            if(input.x == 0 && input.y == 0){
                gameObject.GetComponent<ToolHandler>().changeAnimationState("Idle");
            }else{
                gameObject.GetComponent<ToolHandler>().changeAnimationState("Walking");
            }
        }
    }

    public void setIsAttacking(bool value){
        isAttacking = value;
    }

    public void setCanWalk(bool value){
        canWalk = value;
    }

    

    private void OnDisable() {
        if(!IsOwner) return;
        onFoot.Disable();
    }
}
