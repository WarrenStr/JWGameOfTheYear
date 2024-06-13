using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RbPlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject camParent;

    public float speed, sensitivity, maxForce, jumpForce;
    private float lookRotation;

    private Vector2 move, look;

    public bool isGrounded;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Movement
    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        Look();
    }

    // Input References
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }


    // Movement Functions
    private void Move()
    {
        // find the target velocity
        Vector3 currentVelocity = rb.velocity;

        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        // align direction
        targetVelocity = transform.TransformDirection(targetVelocity);


        // Calculate Force
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z); // gravity calculation

        // Limit Force
        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Look()
    {
        // Turn
        transform.Rotate(Vector3.up * look.x * sensitivity);

        // Look
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camParent.transform.eulerAngles = new Vector3(lookRotation, camParent.transform.eulerAngles.y, camParent.transform.eulerAngles.z);
    }

    private void Jump()
    {
        Vector3 jumpForces = Vector3.zero;

        if (isGrounded)
        {
            jumpForces = Vector3.up * jumpForce;
        }

        rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }

    public void SetIsGrounded(bool state)
    {
        isGrounded = state;
    }

}
