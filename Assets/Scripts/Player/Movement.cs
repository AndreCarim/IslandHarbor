using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Movement : NetworkBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = .4f;

    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isGrounded;

    private bool canWalk = true;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner){return;}

        // Check if the character is grounded by checking for any collider
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(canWalk == true){
            

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void setCanWalk(bool value){
        canWalk = value;
    }
        
}