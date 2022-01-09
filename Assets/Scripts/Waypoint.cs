using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public List<Waypoint> NextWaypoints;
    public GameObject DebugMesh;
    public TravelDirection TravelDirection;

    public TrafficLight BoundTrafficLight;

    // Start is called before the first frame update
    void Start()
    {
        if(DebugMesh != null)
        {
            DebugMesh.SetActive(false);
        }
    }
}

public enum TravelDirection { 
    Inbound,
    Outbound
}