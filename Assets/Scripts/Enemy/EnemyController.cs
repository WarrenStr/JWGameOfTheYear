using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform player;
    private Transform enemyTransform;
    private NavMeshAgent enemyAgent;

    public float ignoreRange = 0;
    public float detectRange = 10;
    public float rotationSpeed = 5;

    public float randomPointRadius = 10;
    private Vector3 randomDestination;
    private bool isMovingToRandomDestination = false;
    public float destinationReachedThreshold = 1.0f;

    // Variables for force threshold and freeze duration
    public float forceThreshold = 10f;
    public float freezeDuration = 3f;

    // Variables to keep track of the enemy's state
    private bool isFrozen = false;
    private Rigidbody enemyRigidbody;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyTransform = GetComponent<Transform>();
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EnemyMovement();
    }

    public void EnemyMovement()
    {
        float distance = Vector3.Distance(enemyTransform.position, player.position);

        if(distance <= detectRange)
        {
            EnemyLookAtPlayer();
            enemyAgent.SetDestination(player.position);
        }
        else if(distance > detectRange)
        {
            if(!isMovingToRandomDestination && enemyAgent.remainingDistance< destinationReachedThreshold )
            {
                GenerateRandomDestination();
            }
        }
    }

    private void EnemyLookAtPlayer()
    {
        enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation,
                Quaternion.LookRotation(player.position - enemyTransform.position), rotationSpeed * Time.deltaTime);
    }

    private void GenerateRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * randomPointRadius;
        NavMeshHit hit;

        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, randomPointRadius, NavMesh.AllAreas);

        randomDestination = hit.position;
        enemyAgent.SetDestination(randomDestination);

        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            // Calculate the force of the collision
            float collisionForce = collision.relativeVelocity.magnitude * collision.rigidbody.mass;

            // Check if the collision force exceeds the threshold
            if (collisionForce >= forceThreshold)
            {
                // Start the freeze coroutine
                StartCoroutine(FreezeEnemy());
            }
        }
    }

    // Coroutine to freeze the enemy
    IEnumerator FreezeEnemy()
    {
        if (!isFrozen)
        {
            isFrozen = true;
            // Disable the enemy's rigidbody physics
            enemyRigidbody.isKinematic = true;

            // Wait for the freeze duration
            yield return new WaitForSeconds(freezeDuration);

            // Re-enable the enemy's rigidbody physics
            enemyRigidbody.isKinematic = false;
            isFrozen = false;
        }
    }
}
