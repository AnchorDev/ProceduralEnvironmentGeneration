using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeedX = 2f;
    public float lookSpeedY = 2f;
    public float upDownSpeed = 5f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        transform.Rotate(Vector3.up * mouseX, Space.Self);
        
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);
        
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        float upDown = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            upDown = upDownSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            upDown = -upDownSpeed * Time.deltaTime;
        }
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal + transform.up * upDown;
        transform.position += moveDirection;
    }
}