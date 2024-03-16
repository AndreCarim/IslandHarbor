using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputMovement : NetworkBehaviour
{
    [SerializeField] private GameObject playerVisual;
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

    [SerializeField] private Animator toolAnimator;
    private string currentAnimationState;







    public override void OnNetworkSpawn(){
        if(!IsOwner) return;

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        DisableRenderersRecursively(playerVisual);


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

            changeAnimationState("Jump");
        }
    }

    private void setAnimation(Vector2 input)
    {
        if (!isAttacking)
        {
            if (input.magnitude == 0)
            {
                // Player idle
                gameObject.GetComponent<ToolHandler>().changeAnimationState("Idle");
                changeAnimationState("Idle");
            }
            else
            {
                // Calculate the angle between the input vector and the forward direction
                float angle = Vector2.SignedAngle(Vector2.up, input.normalized);

                // Determine movement direction based on angle
                if (angle >= -45f && angle <= 45f)
                {
                    // Player walking forward
                    changeAnimationState("RunForward");
                }
                else if (angle > 45f && angle <= 135f)
                {
                    // Player walking right
                    changeAnimationState("RunRight");
                }
                else if (angle > 135f || angle <= -135f)
                {
                    // Player walking backward
                    changeAnimationState("RunBackward");
                }
                else if (angle > -135f && angle <= -45f)
                {
                    // Player walking left
                    
                    changeAnimationState("RunLeft");
                }
                gameObject.GetComponent<ToolHandler>().changeAnimationState("Walking");
            }
        }
    }

    private void changeAnimationState(string newState){
        //stop the same animation from interrupting itself
        if (currentAnimationState == newState || !IsOwner) {return;}

        //Play the animation
        currentAnimationState = newState;
        
        toolAnimator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    public void setIsAttacking(bool value){
        isAttacking = value;
    }

    public void setCanWalk(bool value){
        canWalk = value;
    }

     private void DisableRenderersRecursively(GameObject obj)
    {
        // Disable renderers in the current object
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Disable renderers in child objects recursively
        foreach (Transform child in obj.transform)
        {
            DisableRenderersRecursively(child.gameObject);
        }
    }

    private void OnDisable() {
        if(!IsOwner) return;
        onFoot.Disable();
    }
}
