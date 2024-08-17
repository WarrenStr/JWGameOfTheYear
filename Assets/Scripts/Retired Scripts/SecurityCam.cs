using UnityEngine;

public class SecurityCam : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float centerRadius = 10.0f; // Radius for the main cone
    [SerializeField] private float centerAngle = 45.0f; // Angle of the main cone in degrees
    [SerializeField] private Transform player; // Make player variable serialized

    [SerializeField] private bool showColliders = true; // Toggle for visualizing the colliders
    [SerializeField] private bool showCones = true; // Toggle for visualizing the cones

    private bool playerDetected = false;

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] hitsMain = Physics.OverlapSphere(transform.position, centerRadius, playerLayer);
        playerDetected = false;

        for (int i = 0; i < hitsMain.Length; i++)
        {
            if (hitsMain[i].transform == player)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer <= centerAngle / 2)
                {
                    Debug.Log("Player detected in main cone!");
                    playerDetected = true;
                    return;
                }
            }
        }
    }

    // Optional: Visualize the detection radius and cones in the editor
    void OnDrawGizmos()
    {
        if (showColliders)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, centerRadius);
        }

        if (showCones)
        {
            // Draw main detection cone
            Gizmos.color = playerDetected ? Color.red : Color.green;
            Vector3 leftBoundaryMain = Quaternion.Euler(0, -centerAngle / 2, 0) * transform.forward * centerRadius;
            Vector3 rightBoundaryMain = Quaternion.Euler(0, centerAngle / 2, 0) * transform.forward * centerRadius;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundaryMain);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundaryMain);

            DrawArc(transform.position, transform.forward, centerRadius, -centerAngle / 2, centerAngle / 2);
        }
    }

    void DrawArc(Vector3 center, Vector3 forward, float radius, float startAngle, float endAngle, int segments = 20)
    {
        float angleStep = (endAngle - startAngle) / segments;
        Vector3 previousPoint = center + Quaternion.Euler(0, startAngle, 0) * forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            Vector3 nextPoint = center + Quaternion.Euler(0, currentAngle, 0) * forward * radius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
