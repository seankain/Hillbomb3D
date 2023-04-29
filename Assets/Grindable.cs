using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grindable : MonoBehaviour
{

    [SerializeField]
    private Transform[] Waypoints;
    //TODO AudioSource for the sound it makes when trucks or board are touching

    public bool TryGetNextWaypoint(Vector3 position, out Transform nextPosition)
    {
        for(var i = 0; i < Waypoints.Length; i++)
        {
            if(position.z < Waypoints[i].position.z)
            {
                nextPosition = Waypoints[i];
                return true;
            }
        }
        //There's no more waypoints so indicate grind is over
        nextPosition = null;
        return false;
    }
    
    public Vector3 GetInitPosition(Vector3 position)
    {

        var prevWaypoint = Waypoints[0];
        Transform currentWaypoint = null;
        for (var i = 0; i < Waypoints.Length; i++)
        {
            if (position.z < Waypoints[i].position.z)
            {
                currentWaypoint = Waypoints[i];
                
            }
            else
            {
                prevWaypoint = Waypoints[i];
            }
        }
        return Vector3.Lerp(prevWaypoint.position,currentWaypoint.position,Vector3.Distance(position,currentWaypoint.position)/Vector3.Distance(prevWaypoint.position,currentWaypoint.position));
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
