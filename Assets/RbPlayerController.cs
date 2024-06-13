using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RbPlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject camParent;

    public float speed, sensitivity, maxForce;
    private float lookRotation;

    private Vector2 move, look;

    // Input References
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    // Movement
    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        // find the target velocity
        Vector3 currentVelocity = rb.velocity;

        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        // align direction
        targetVelocity = transform.TransformDirection(targetVelocity);


        // Calculate Force
        Vector3 velocityChange = (targetVelocity - currentVelocity);

        // Limit Force
        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

}
