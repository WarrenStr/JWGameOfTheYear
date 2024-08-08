using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour
{
    public float rotationSpeed = 30.0f; // Degrees per second
    public float maxRotationAngle = 45.0f; // Maximum angle from the starting point

    private float startAngle;
    private float currentAngle;
    private bool rotatingRight = true;

    void Start()
    {
        startAngle = transform.eulerAngles.y;
        currentAngle = startAngle;
    }

    public void ScanRoom()
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

    public void FollowPlayer(Transform player)
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        if (Mathf.Abs(angleDifference) > maxRotationAngle)
        {
            // Stop following if player is out of bounds
            return;
        }

        float rotationStep = rotationSpeed * Time.deltaTime;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationStep);

        // Cap the rotation to the max left and right rotation angles
        newAngle = Mathf.Clamp(newAngle, startAngle - maxRotationAngle, startAngle + maxRotationAngle);

        currentAngle = newAngle;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
    }
}
