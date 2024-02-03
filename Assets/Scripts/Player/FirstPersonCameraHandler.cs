using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class FirstPersonCameraHandler : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float smoothTime = 0.1f; // Smoothing time for camera movement

    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private Camera firstPersonCamera;

    private float xRotation = 0f;
    private Vector3 smoothVelocity; // Velocity for smooth camera movement

    private bool isFreeToLook = true;

      private bool isMainCamera = false; // Flag to determine if this camera should act as the main camera

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        initialize();
    }

    void Update()
    {
        if (!isMainCamera || !IsOwner)
        {
            return; // Do not update if this camera is not the main camera
        }

        if (isFreeToLook)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, 0f);
            firstPersonCamera.transform.localRotation = Quaternion.Lerp(firstPersonCamera.transform.localRotation, targetRotation, smoothTime); // Smooth rotation

            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    public void setIsFreeToLook(bool value)
    {
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

    public void setOnStart( GameObject entirePlayerUIInstance){

    }
}

