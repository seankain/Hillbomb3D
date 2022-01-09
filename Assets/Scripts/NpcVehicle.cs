using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcVehicle : MonoBehaviour
{
    public float currentTorque = 0f;
    public float Acceleration = 1f;
    public float wayPointDistanceThreshold = 5f;
    public Transform ForwardCastLocation;
    public float ForwardCastDistance = 10f;
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float maxSpeedMetersPerSecond = 13;
    private bool[] wheelGroundedState;
    private WheelCollider[] wheelColliders;
    private Rigidbody rb;
    private VehicleDriveState driveState;


    public float extremumSlip;
    public float extremumValue;
    public float asymptoteSlip;
    public float asymptoteValue;
    public float stiffness;

    public HillChunk CurrentChunk;
    public float CurrentMotor { get { return motor; } }
    public float CurrentBrake { get { return brake; } }
    private float motor = 0;
    private float brake = 0;
    private float steering = 0;
    private GameObject[] waypoints;
    private Waypoint activeWaypoint;
    private WheelHit[] wheelHits;
    private ChunkCycler chunkCycler;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheelColliders = GetComponentsInChildren<WheelCollider>();
        wheelGroundedState = new bool[wheelColliders.Length];
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(w => w.transform.position.z).ToArray();
        activeWaypoint = GetNextWayPoint();
        wheelHits = new WheelHit[4];
        driveState = VehicleDriveState.Driving;
        chunkCycler = FindObjectOfType<ChunkCycler>();
    }

    public void FixedUpdate()
    {
        if (!gameObject.activeSelf) { return; }

        var obstacleDistance = DistanceFromNextObstruction();
        //obstacle or red light to consider
        if (obstacleDistance < Mathf.Infinity)
        {
            var arrivalTime = obstacleDistance / rb.velocity.z;
            motor = rb.mass * (-rb.velocity.magnitude / arrivalTime);
           // Debug.Log($"Arrival time {arrivalTime} torque {motor}");

        }
        // no obstacle or red light to consider, go to max allowed speed
        else
        {
            if (rb.velocity.magnitude < maxSpeedMetersPerSecond)
            {
                //Debug.Log($"Accelerating. Current motor torque {motor} newton meters. Steering angle {steering}");
                if (motor < maxMotorTorque)
                {
                    motor += Acceleration;
                }
                else
                {
                    motor = maxMotorTorque;
                }
                brake = 0;
            }
            if (rb.velocity.magnitude > maxSpeedMetersPerSecond)
            {
                if (motor > 0)
                {
                    motor -= Acceleration;
                }
                brake += Acceleration;

            }
        }

        //if (driveState == VehicleDriveState.Braking)
        //{
        //    var dist = Vector3.Distance(gameObject.transform.position, activeWaypoint.transform.position);
        //    var arrivalTime = dist / rb.velocity.magnitude;
        //    brake = rb.mass * arrivalTime;
        //}
        //if (driveState == VehicleDriveState.Stopped)
        //{
        //    return;
        //}
        steering = GetNextSteeringAngle();
        foreach (AxleInfo axleInfo in axleInfos)
        {
            //    // This should likely do more per wheel given that wheels will have different forces applied if they arent all touching etc
            //    //axleInfo.leftWheel.forwardFriction = AllWheelForwardFriction;
            //    //axleInfo.rightWheel.forwardFriction = AllWheelForwardFriction;
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
        }


    }

    /// <summary>
    /// Searches forward for obstructions and looks for imminent red lights and returns the closer one, infinity for nothing
    /// </summary>
    /// <returns>Distance in meters to obstruction requiring reaction</returns>
    private float DistanceFromNextObstruction()
    {
        RaycastHit hit;
        TrafficLight activeTrafficLight = null;
        var obstruction = (Physics.SphereCast(ForwardCastLocation.position, 0.2f, ForwardCastLocation.forward, out hit, ForwardCastDistance,~1<<10));

        if (activeWaypoint.BoundTrafficLight != null && activeWaypoint.BoundTrafficLight.CurrentState == TrafficLightState.LightStop)
        {
            activeTrafficLight = activeWaypoint.BoundTrafficLight;
        }
        //Red light, no obstruction
        if (!obstruction && activeTrafficLight != null)
        {
            return Vector3.Distance(gameObject.transform.position, activeTrafficLight.gameObject.transform.position);
        }
        // Both redlight and obstruction, return closer
        if (obstruction && activeTrafficLight != null)
        {
            Debug.Log($"{gameObject.name} seeing {hit.collider.gameObject.name}");
            return Mathf.Min(Vector3.Distance(gameObject.transform.position, activeTrafficLight.gameObject.transform.position),
                Vector3.Distance(gameObject.transform.position, hit.transform.position));
        }
        //Obstruction but no red light
        if (obstruction && activeTrafficLight == null)
        {
            return Vector3.Distance(gameObject.transform.position, hit.transform.position);
        }
        //No red light, no obstruction
        return float.PositiveInfinity;
    }

    private float GetNextSteeringAngle()
    {
        var target = activeWaypoint.transform.position;
        var targetDir = target - transform.position;
        return -Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
    }

    private Waypoint GetNextWayPoint()
    {

        var closest = waypoints[0];
        var distance = Vector3.Distance(gameObject.transform.position, closest.transform.position);
        for (var i = 1; i < waypoints.Length; i++)
        {
            var curDistance = Vector3.Distance(gameObject.transform.position, waypoints[i].transform.position);
            if (curDistance < distance)
            {
                closest = waypoints[i];
                distance = curDistance;
            }

        }
        return closest.GetComponent<Waypoint>();
    }

    public void Update()
    {
        if (!gameObject.activeSelf) { return; }
        //Player passed by just re-enter the pool
        if (CurrentChunk.Passed) { gameObject.SetActive(false); return; }
        for (var i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].forwardFriction = new WheelFrictionCurve
            {
                asymptoteSlip = asymptoteSlip,
                asymptoteValue = asymptoteValue,
                extremumSlip = extremumSlip,
                extremumValue = extremumValue,
                stiffness = stiffness
            };
            wheelGroundedState[i] = wheelColliders[i].isGrounded;
            if (wheelColliders[i].GetGroundHit(out var hit))
            {
                wheelHits[i] = hit;
            }
        }
    }

    public WheelHit[] GetWheelHitInfo()
    {
        return wheelHits;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            var enteredWaypoint = other.GetComponent<Waypoint>();
            // hitting intersecting waypoints
            if (enteredWaypoint != activeWaypoint) { return; }
            if(enteredWaypoint.NextWaypoints.Count < 1)
            {
                //Waypoints with no next waypoint were causing helpless zombie vehicles
                //this might make them visibly vanish if the waypoints are misconfigured
                //but would otherwise act like a sink
                gameObject.SetActive(false);
                return;
            }
            activeWaypoint = other.GetComponent<Waypoint>().NextWaypoints[Random.Range(0, enteredWaypoint.NextWaypoints.Count - 1)];
            //Consider changing all waypoint game object references to actual waypoint now that it holds more information
            //var nextWayPoint = activeWaypoint.GetComponent<Waypoint>();
            //if (nextWayPoint.BoundTrafficLight != null && nextWayPoint.BoundTrafficLight.CurrentState == TrafficLightState.LightStop)
            //{
            //    BrakeToStop();
            //}
           // Debug.Log($"Passing {other.gameObject.name}, traveling to {activeWaypoint.gameObject.name}");
        }
        //Leaving traffic, go back into pool
        if (other.gameObject.tag == "NpcSink")
        {
            gameObject.SetActive(false);
        }
        if (other.gameObject.tag == "ChunkStart")
        {
            var hillChunk = other.gameObject.GetComponentInParent<HillChunk>();
            //is this just the start to the chunk we've been in
            if (hillChunk == CurrentChunk && chunkCycler.TryGetNeighborChunk(CurrentChunk,activeWaypoint.TravelDirection,out var neighborChunk))
            {
                CurrentChunk = neighborChunk;
            }
            else
            {
                CurrentChunk = hillChunk;
            }
            
            if (activeWaypoint.TravelDirection == TravelDirection.Outbound)
            {
                //Go to the bottom of the chunk further back in z axis
                activeWaypoint = CurrentChunk.OutboundBottomWaypoint;
            }
            else
            {
                //start at top of next chunk like normal
                activeWaypoint = CurrentChunk.InboundTopWaypoint;
            }
        }
        if(other.gameObject.tag == "ChunkEnd")
        {
            var hillChunk = other.gameObject.GetComponentInParent<HillChunk>();
            CurrentChunk = hillChunk;
            activeWaypoint = hillChunk.OutboundBottomWaypoint;
            if (activeWaypoint.TravelDirection == TravelDirection.Outbound)
            {
                activeWaypoint = hillChunk.OutboundBottomWaypoint;
            }
            else
            {
                if (chunkCycler.TryGetNeighborChunk(CurrentChunk, activeWaypoint.TravelDirection, out var neighborChunk))
                {
                    activeWaypoint = hillChunk.InboundTopWaypoint;
                }
                else {
                    gameObject.SetActive(false);
                    Debug.Log("No neighbor chunk, just go inactive and prepare for recycling");
                }
                
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.other.tag == "Respawn")
        {
            gameObject.SetActive(false);
        }
    }

    private void Stop()
    {
        driveState = VehicleDriveState.Stopped;
    }

    private void BrakeToStop()
    {
        Debug.Log("Braking to stop");
        driveState = VehicleDriveState.Braking;
    }

    private void Go()
    {
        driveState = VehicleDriveState.Driving;
    }

}

public enum VehicleDriveState
{
    Driving,
    Stopped,
    Braking
}


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
