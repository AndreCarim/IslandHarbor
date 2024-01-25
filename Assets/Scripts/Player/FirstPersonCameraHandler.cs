using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraHandler : MonoBehaviour
{
    //
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;

    private bool isFreeToLook = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFreeToLook == true){
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    public void setIsFreeToLook(bool value){
        isFreeToLook = value;

        if(value == true){
            //lock the cursor
            Cursor.lockState = CursorLockMode.Locked;
        }else{
            //unlock cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
