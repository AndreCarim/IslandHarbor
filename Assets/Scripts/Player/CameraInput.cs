using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraInput : NetworkBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private bool isMainCamera = false; // Flag to determine if this camera should act as the main camera
    private bool isFreeToLook = true;

    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private Camera firstPersonCamera;
    private float xRotation = 0f;

    public float xSensitivity = 50f;
    public float ySensitivity = 50f;


    public override void OnNetworkSpawn(){
        if(!IsOwner)return;

        base.OnNetworkSpawn();
        initialize();

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        Cursor.lockState = CursorLockMode.Locked;

        onFoot.Enable();
    }

    private void initialize()
    {
        cameraHolder.SetActive(true);

        // Check if this camera belongs to the local player
        if (IsOwner)
        {
            isMainCamera = true;
            firstPersonCamera.enabled = true; // Enable the camera for the local player
            setIsFreeToLook(true); // Allow camera movement for the local player
        }
        else
        {
            firstPersonCamera.enabled = false; // Disable the camera for remote players
            setIsFreeToLook(false); // Prevent camera movement for remote players
        }
    }

    private void LateUpdate(){
        if(!isMainCamera || !IsOwner)return;

        ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    public void ProcessLook(Vector2 input){
        if(isFreeToLook){
            float mouseX = input.x;
            float mouseY = input.y;

            xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
        }   
    }

    public void setIsFreeToLook(bool value)
    {
        if(!IsOwner)return;

        isFreeToLook = value;

        if (value)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    

     private void OnDisable() {
        if(!IsOwner) return;
        Cursor.lockState = CursorLockMode.None;
        onFoot.Disable();
    }
}
