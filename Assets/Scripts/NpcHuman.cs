using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcHuman : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    [SerializeField]
    private Ragdoller ragdoller;
    [SerializeField]
    private float waypointDistanceMax = 10f;
    [SerializeField]
    private float waypointDistanceMin = 1f;
    [SerializeField]
    private float speed = 1f;
    private Vector3 currentDestination;
    

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.SetDestination(SelectRandomWayPoint());
        
    }

    private Vector3 SelectRandomWayPoint() 
    {
        currentDestination = new Vector3(
            gameObject.transform.position.x + Random.Range(-waypointDistanceMax, waypointDistanceMax),
            gameObject.transform.position.y,
            gameObject.transform.position.z + Random.Range(-waypointDistanceMax, waypointDistanceMax));
        return currentDestination;
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.transform.LookAt(currentDestination);
        if (navMeshAgent.remainingDistance < 1)
        {
            navMeshAgent.SetDestination(SelectRandomWayPoint());
        }
    }
}
