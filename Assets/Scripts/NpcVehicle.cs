using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcVehicle : MonoBehaviour
{
    public float currentTorque = 0f;
    public float Acceleration = 1f;
    public float wayPointDistanceThreshold = 5f;
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

    public float CurrentMotor { get { return motor; } }
    public float CurrentBrake { get { return brake; } }
    private float motor = 0;
    private float brake = 0;
    private float steering = 0;
    private GameObject[] waypoints;
    private Waypoint activeWaypoint;
    private WheelHit[] wheelHits;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheelColliders = GetComponentsInChildren<WheelCollider>();
        wheelGroundedState = new bool[wheelColliders.Length];
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(w => w.transform.position.z).ToArray();
        activeWaypoint = GetNextWayPoint();
        wheelHits = new WheelHit[4];
    }

    public void FixedUpdate()
    {
        if (!gameObject.activeSelf) { return; }
        if (driveState == VehicleDriveState.Driving)
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
        if (driveState == VehicleDriveState.Braking)
        {
            var dist = Vector3.Distance(gameObject.transform.position, activeWaypoint.transform.position);
            var arrivalTime = dist / rb.velocity.magnitude;
            brake = rb.mass * arrivalTime;
        }
        if(driveState == VehicleDriveState.Stopped)
        {
            return;
        }
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
            if(enteredWaypoint != activeWaypoint) { return; }
            activeWaypoint = other.GetComponent<Waypoint>().NextWaypoints[Random.Range(0, enteredWaypoint.NextWaypoints.Count - 1)];
            //Consider changing all waypoint game object references to actual waypoint now that it holds more information
            var nextWayPoint = activeWaypoint.GetComponent<Waypoint>();
            if (nextWayPoint.BoundTrafficLight != null && nextWayPoint.BoundTrafficLight.CurrentState == TrafficLightState.LightStop)
            {
                BrakeToStop();
            }
            Debug.Log($"Passing {other.gameObject.name}, traveling to {activeWaypoint.gameObject.name}");
        }
        //Leaving traffic, go back into pool
        if (other.gameObject.tag == "NpcSink")
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
