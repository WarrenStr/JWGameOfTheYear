using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    public float grabRange = 3f; // Range of the raycast for grabbing objects
    public LayerMask grabLayerMask; // Layer mask to specify which objects can be grabbed
    public Transform holdPosition; // Position in front of the player where the object will be held
    public float releaseForce = 10f; // Force applied to the object when it is released

    private Camera playerCamera; // Reference to the player's camera
    private Rigidbody grabbedObjectRb; // Reference to the currently grabbed object's Rigidbody
    private bool isGrabbing; // Boolean to track if the player is currently grabbing an object
    private bool isHolding; // Boolean to track if the player is currently holding an object

    private PlayerInput playerInput; // Reference to the PlayerInput component
    private InputAction grabAction; // Reference to the grab action

    private Collider playerCollider; // Reference to the player's collider
    private Collider grabbedObjectCollider; // Reference to the grabbed object's collider

    private void Awake()
    {
        playerCamera = Camera.main; // Get the main camera
        playerInput = GetComponent<PlayerInput>(); // Get the PlayerInput component
        grabAction = playerInput.actions["Grab"]; // Get the grab action
        playerCollider = GetComponent<Collider>(); // Get the player's collider
    }

    private void OnEnable()
    {
        grabAction.performed += OnGrab; // Subscribe to the performed event of the grab action
    }

    private void OnDisable()
    {
        grabAction.performed -= OnGrab; // Unsubscribe from the performed event of the grab action
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        if (isGrabbing)
        {
            ReleaseObject(); // Release the currently grabbed object
        }
        else
        {
            TryGrabObject(); // Try to grab a new object
        }
    }

    private void TryGrabObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Create a ray from the center of the screen
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange, grabLayerMask))
        {
            if (hit.collider != null && hit.collider.attachedRigidbody != null)
            {
                grabbedObjectRb = hit.collider.attachedRigidbody; // Get the Rigidbody of the hit object
                grabbedObjectRb.isKinematic = true; // Disable physics on the grabbed object
                grabbedObjectCollider = hit.collider; // Get the collider of the grabbed object
                Physics.IgnoreCollision(playerCollider, grabbedObjectCollider, true); // Ignore collision between player and grabbed object
                grabbedObjectRb.transform.position = holdPosition.position; // Move the object to the hold position
                grabbedObjectRb.transform.parent = holdPosition; // Set the object's parent to the hold position
                isHolding = true;
                isGrabbing = true; // Set the isGrabbing flag to true
            }
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObjectRb != null)
        {
            grabbedObjectRb.isKinematic = false; // Re-enable physics on the object
            grabbedObjectRb.transform.parent = null; // Detach the object from the hold position
            Physics.IgnoreCollision(playerCollider, grabbedObjectCollider, false); // Re-enable collision between player and grabbed object

            // Add force to the released object
            Vector3 releaseDirection = playerCamera.transform.forward; // Use the forward direction of the camera
            grabbedObjectRb.AddForce(releaseDirection * releaseForce, ForceMode.VelocityChange);

            grabbedObjectRb = null; // Clear the reference to the grabbed object's Rigidbody
            grabbedObjectCollider = null; // Clear the reference to the grabbed object's collider
            isHolding = false;
            isGrabbing = false; // Set the isGrabbing flag to false
        }
    }

    private void Update()
    {
        if (isGrabbing && isHolding && grabbedObjectRb != null)
        {
            grabbedObjectRb.transform.position = holdPosition.position; // Continuously move the object to the hold position
            Debug.Log(grabbedObjectRb.transform);
            isHolding = false;
        }
    }
}
