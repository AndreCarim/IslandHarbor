using UnityEngine;

public class FirstPersonCameraHandler : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float smoothTime = 0.1f; // Smoothing time for camera movement

    private float xRotation = 0f;
    private Vector3 smoothVelocity; // Velocity for smooth camera movement

    private bool isFreeToLook = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isFreeToLook)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, smoothTime); // Smooth rotation

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
}

