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

    private float xSensitivity;
    private float ySensitivity;

    private const string MOUSE_SENSITIVITY_KEY = "MouseSensitivity";


    public override void OnNetworkSpawn(){
        if(!IsOwner)return;

        base.OnNetworkSpawn();
        initialize();

        float savedSensitivity = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEY, 5f);
        setMouseSensitivity(savedSensitivity);

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
        
        if (value)
        {
            // Hide and lock the cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Make the cursor visible and unlock it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        isFreeToLook = value;
    }

    

     private void OnDisable() {
        if(!IsOwner) return;
        Cursor.lockState = CursorLockMode.None;
        onFoot.Disable();
    }

    public void setMouseSensitivity(float newValue){
        if(!IsOwner)return;
        xSensitivity = newValue;
        ySensitivity = newValue;

        // Save the mouse sensitivity value to PlayerPrefs
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_KEY, newValue);
        PlayerPrefs.Save(); // Save the PlayerPrefs data immediately
    }

    public float getMouseSensitivy(){
        return xSensitivity;
    }
}
