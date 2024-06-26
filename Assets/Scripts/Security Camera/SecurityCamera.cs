using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    // Rotation settings
    public float rotationSpeed = 30.0f; // Degrees per second
    public float maxRotationAngle = 45.0f; // Maximum angle from the starting point
    public LayerMask playerLayer;
    public float detectionRadius = 10.0f;
    public Transform player; // Make player variable public

    private float startAngle;
    private float currentAngle;
    private bool rotatingRight = true;
    private bool playerDetected = false;

    void Start()
    {
        startAngle = transform.eulerAngles.y;
        currentAngle = startAngle;
    }

    void Update()
    {
        DetectPlayer();
        if (playerDetected)
        {
            FollowPlayer();
        }
        else
        {
            ScanRoom();
        }
    }

    void ScanRoom()
    {
        float angleChange = rotationSpeed * Time.deltaTime;
        if (rotatingRight)
        {
            currentAngle += angleChange;
            if (currentAngle >= startAngle + maxRotationAngle)
            {
                rotatingRight = false;
            }
        }
        else
        {
            currentAngle -= angleChange;
            if (currentAngle <= startAngle - maxRotationAngle)
            {
                rotatingRight = true;
            }
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        playerDetected = false;
        foreach (var hit in hits)
        {
            if (hit.transform == player)
            {
                Debug.Log("Player detected!");
                playerDetected = true;
                break;
            }
        }
    }

    void FollowPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        if (Mathf.Abs(angleDifference) > maxRotationAngle)
        {
            playerDetected = false; // Stop following if player is out of bounds
            return;
        }

        float rotationStep = rotationSpeed * Time.deltaTime;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationStep);

        // Cap the rotation to the max left and right rotation angles
        newAngle = Mathf.Clamp(newAngle, startAngle - maxRotationAngle, startAngle + maxRotationAngle);

        currentAngle = newAngle;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
    }

    // Optional: Visualize the detection radius in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
