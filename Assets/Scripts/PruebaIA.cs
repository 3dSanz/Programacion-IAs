using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PruebaIA : MonoBehaviour
{
    enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }

    State _currentState;
    NavMeshAgent _agent;
    Transform _player;
    [SerializeField] float _detectionRange = 15;
    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] float _attackRange = 5;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        PuntoAleatorio();
        _currentState = State.Patrolling;
    }

    void Update()
    {
        switch (_currentState) 
        {
            case State.Patrolling:
                Patrulla();
            break;
            case State.Chasing:
                Perseguir();
            break;
            case State.Attacking:
                Atacar();
            break;
        }
    }

    void Patrulla()
    {
        if(EnRango(_detectionRange) == true)
        {
            _currentState = State.Chasing;
        }

        if(_agent.remainingDistance < 0.5f)
        {
            PuntoAleatorio();
        }
    }

    void Perseguir()
    {
        _agent.destination = _player.position;
        if(EnRango(_detectionRange) == false)
        {
            _currentState = State.Patrolling;
        }

        if(EnRango(_attackRange) == true)
        {
            _currentState = State.Attacking;
        }
    }

    void Atacar()
    {
        Debug.Log("Ataque!");
        _currentState = State.Chasing;
    }

    void PuntoAleatorio()
    {
        _agent.destination = _patrolPoints[Random.Range(0,_patrolPoints.Length)].position;
    }

    bool EnRango(float _rango)
    {
        if(Vector3.Distance(transform.position, _player.position) < _rango)
        {
            return true;
        } else
        {
            return false;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach(Transform _point in _patrolPoints)
        {
            Gizmos.DrawWireSphere(_point.position, 0.5f);
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
