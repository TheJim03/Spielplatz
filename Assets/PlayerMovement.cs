// PlayerMovement.cs  (an den Ghost hängen)
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;

    CharacterController controller;
    Transform cam;
    float turnSmoothVelocity;
    Vector3 velocity;

    void Start(){
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update(){
        // Boden/Schwerkraft
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f){
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z)*Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}