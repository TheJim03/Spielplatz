using UnityEngine;
using Unity.Cinemachine;

public class FirstPersonLook : MonoBehaviour
{
    public CinemachineCamera cam;
    public float sensX = 150f;
    public float sensY = 150f;

    float rotX;
    float rotY;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(rotX, rotY, 0f);
    }
}
