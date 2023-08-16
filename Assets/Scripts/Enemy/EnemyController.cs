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

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        enemyTransform = GetComponent<Transform>();
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
}
