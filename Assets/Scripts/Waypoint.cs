using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public List<Waypoint> NextWaypoints;
    public GameObject DebugMesh;
    public Vector3 TravelDirection;

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
