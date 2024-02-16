using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemy : MonoBehaviour
{

    enum State
    {
        Patrolling,
        Waiting,
        Chasing,
        Attacking,
        Searching
    }

    State currentState;

    NavMeshAgent enemyAgent;
    Transform playerTransform;

    [SerializeField] Transform patrolAreaCenter;
    [SerializeField] Vector2 patrolAreaSize;
    [SerializeField] float visionRange = 15;
    [SerializeField] float visionAngle = 90;

    Vector3 lastTargetPosition;

    float searchTimer;
    [SerializeField] float searchWaitTime = 15;
    [SerializeField] float searchRadius = 30;

    [SerializeField] float patrolTimer;
    [SerializeField] float patrolWaitTime = 5;
    [SerializeField] Transform[] spot;

    [SerializeField] float attackRange = 5;
    [SerializeField] int patrollingSpot = 0;

    // Start is called before the first frame update
    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        enemyAgent.destination = spot[0].position;
         if(enemyAgent.remainingDistance < 0.5f)
        {
            currentState = State.Waiting;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState) 
        {
            case State.Patrolling:
                Patrol();
            break;
            case State.Waiting:
                Wait();
            break;
            case State.Chasing:
                Chase();
            break;
            case State.Attacking:
                Attack();
            break;
            case State.Searching:
                Search();
            break;
        }
    }

    void Search()
    {
        if(OnRange() == true)
        {
            searchTimer = 0;
            currentState = State.Chasing;
        }

        searchTimer += Time.deltaTime;

        if(searchTimer < searchWaitTime)
        {
            if(enemyAgent.remainingDistance < 0.5f)
            {
                Debug.Log("Buscando punto aleatorio");
                Vector3 randomSearchPoint = lastTargetPosition + Random.insideUnitSphere * searchRadius;
                randomSearchPoint.y = lastTargetPosition.y;
                enemyAgent.destination = randomSearchPoint;
            }

        }else
        {
            currentState = State.Patrolling;
        }
    }

    void Patrol()
    {
        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }

        if(enemyAgent.remainingDistance < 0.5f)
        {
            currentState = State.Waiting;
            SetPosition();
        }

    }

    void Attack()
    {
        Debug.Log("Atacado!");
        currentState = State.Chasing;
    }

    void Chase()
    {
        enemyAgent.destination = playerTransform.position;

        if(OnAttackRange() == true)
        {
            currentState = State.Attacking;
        }

        if(OnRange() == false)
        {
            currentState = State.Searching;
        }
    }

    void Wait()
    {
        if(enemyAgent.remainingDistance < 0.5f)
        {
        patrolTimer += Time.deltaTime;
        if(patrolTimer >= patrolWaitTime)
            {
                currentState = State.Patrolling;
                patrolTimer = 0;
            }
        }

        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }
    }

    /*void SetRandomPoint()
    {
        float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
        float randomZ = Random.Range(-patrolAreaSize.y / 2, patrolAreaSize.y / 2);
        Vector3 randomPoint = new Vector3(randomX, 0f, randomZ) + patrolAreaCenter.position;

        enemyAgent.destination = randomPoint;
    }*/

    void SetPosition()
    {
       /* //Position 1
        if(patrollingSpot == 0)
        {
            enemyAgent.destination = spot[0].position;
        }

        if(transform.position == spot[0].position)
        {
            patrollingSpot++;
        }
        
        //Position 2
        if(patrollingSpot == 1)
        {
            enemyAgent.destination = spot[1].position;
        }

        if(transform.position == spot[1].position)
        {
            patrollingSpot = 3;
        }

        //Position 3
        if(patrollingSpot == 3)
        {
            enemyAgent.destination = spot[2].position;
        }

        if(transform.position == spot[2].position)
        {
            patrollingSpot = 4;
        }
        
        //Position 4
        if(patrollingSpot == 4)
        {
            enemyAgent.destination = spot[3].position;
        }

        if(transform.position == spot[3].position)
        {
            patrollingSpot = 1;
        }*/

        patrollingSpot++;

        if(patrollingSpot > 3)
        {
            patrollingSpot = 0;
        }

        enemyAgent.destination = spot[patrollingSpot].position;
    }

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, playerTransform.position) <= visionRange)
        {
            return true;
        } else
        {
            return false;
        }*/

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(playerTransform.position == lastTargetPosition)
            {
                return true;
            }
            //return true;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    lastTargetPosition = playerTransform.position;
                    return true;
                }
                
                return false;
            }
        }
            return false;
    }

    bool OnAttackRange()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(playerTransform.position == lastTargetPosition)
            {
                return true;
            }
            //return true;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit, attackRange))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    lastTargetPosition = playerTransform.position;
                    return true;
                }
                
                return false;
            }
        }
            return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolAreaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Vector3 fovLine2= Quaternion.AngleAxis(-visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);

    }
}
