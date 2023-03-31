using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastBoardController : MonoBehaviour
{
    public Transform LeftFrontWheel;
    public Transform RightFrontWheel;
    public Transform LeftRearWheel;
    public Transform RightRearWheel;
    public Transform FrontAxle;
    public Transform RearAxle;
    private Transform[] wheels;
    public float PlayerRotateSpeed = 5f;
    public float MaxTurnAngle = 5f;
    public float TurnSpeed = 1f;
    public float RayGroundDistance = 1f;
    public float CastDistance = 0.1f;
    public float SuspensionRestDistance = 0.1f;
    public float Damping = 30f;
    public float Strength = 100f;
    public float Offset = 0.1f;
    public float WheelGripFactor = 0.1f;
    public float WheelMass = 1;
    private Rigidbody rb;
    private float horizontal = 0f;
    private float currentTurnAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = new Transform[4]
        {
            LeftFrontWheel,
            RightFrontWheel,
            LeftRearWheel,
            RightRearWheel
        };
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        //if(currentTurnAngle < MaxTurnAngle)
        //{
        //TODO fix
        currentTurnAngle += horizontal * TurnSpeed * Time.deltaTime;
        //Debug.Log(currentTurnAngle);
        currentTurnAngle = Mathf.Clamp(currentTurnAngle, -MaxTurnAngle, MaxTurnAngle);
        //LeftFrontWheel.localEulerAngles = new Vector3(0, currentTurnAngle, 0);
        //RightFrontWheel.localEulerAngles = new Vector3(0, currentTurnAngle, 0);
        //LeftRearWheel.localEulerAngles = new Vector3(0, -currentTurnAngle, 0);
        //RightRearWheel.localEulerAngles = new Vector3(0, -currentTurnAngle, 0);

        FrontAxle.localEulerAngles = new Vector3(0, currentTurnAngle, 0);
        //RearAxle.localEulerAngles = new Vector3(0, -currentTurnAngle, 0);
        //}

        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
        if (Physics.Raycast(ray, out var hitInfo, 1, ~(1 << LayerMask.NameToLayer("Skateboard"))))
        {
            if (hitInfo.distance < RayGroundDistance)
            {
                // gameObject.transform.position = hitInfo.point + new Vector3(0, RayGroundDistance, 0);
                gameObject.transform.rotation *= Quaternion.FromToRotation(gameObject.transform.up, hitInfo.normal);
                gameObject.transform.Rotate(Vector3.down, -horizontal * PlayerRotateSpeed * Time.deltaTime);
            }
        }
    }

    private void AdjustWheelSuspension(Transform wheel, Vector3 wheelWorldVelocity)
    {
        var ray = new Ray(wheel.position, Vector3.down);
        Debug.DrawRay(wheel.position, Vector3.down, Color.red);
        if (Physics.Raycast(ray, out var hitInfo, CastDistance, ~(1 << LayerMask.NameToLayer("Skateboard"))))
        {
            //Credit to Toyful games from their Very Very Valet tutorial
            // world-space velocity of wheel
            Vector3 springDir = wheel.up;
            //calculate the offset from the raycast
            float offset = SuspensionRestDistance - hitInfo.distance;
            //calculate velocity along the spring direction
            // note that springDir is a unit vector, so this returns the magnitude of wheel world velocity
            // as projected onto spring dir
            float vel = Vector3.Dot(springDir, wheelWorldVelocity);
            //calculate the magnitude of the damped spring force
            float force = (offset * Strength) - (vel * Damping);
            rb.AddForceAtPosition(springDir * force, wheel.position);
        }
    }

    private void AdjustWheelSlip(Transform wheel)
    {
        Vector3 steeringDir = wheel.right;
        //worldspace velocity of the suspension
        Vector3 wheelWorldVel = rb.GetPointVelocity(wheel.position);
        //what is the wheels velocity in the steering direction?
        // note that steeringDir is a unit vector, so this returns the magnitude of wheelWorldVel
        float steeringVel = Vector3.Dot(steeringDir, wheelWorldVel);
        // the change in velocity that we're looking for is -steeringVel*gripFactor
        // grip factor is in range 0-1,0 means no grip, 1 means full grip
        float desiredVelChange = -steeringVel * WheelGripFactor;
        //turn change in velocity into an acceleration
        //this will produce the acceleration necessary to change the velocity by desiredVelChange in 1 physics step
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        rb.AddForceAtPosition(steeringDir * WheelMass * desiredAccel, wheel.position);
    }

    private void AddSteeringForces(Transform wheel)
    {
        var ray = new Ray(wheel.position, Vector3.down * CastDistance);
        Debug.DrawRay(wheel.position, Vector3.down * CastDistance, Color.red);
        if (Physics.Raycast(ray, out var hitInfo, CastDistance, ~(1 << LayerMask.NameToLayer("Skateboard"))))
        {
            //Spring
            Vector3 wheelWorldVel = rb.GetPointVelocity(wheel.position);
            //Credit to Toyful games from their Very Very Valet tutorial
            // world-space velocity of wheel
            Vector3 springDir = wheel.up;
            //calculate the offset from the raycast
            float offset = SuspensionRestDistance - hitInfo.distance;
            //calculate velocity along the spring direction
            // note that springDir is a unit vector, so this returns the magnitude of wheel world velocity
            // as projected onto spring dir
            float vel = Vector3.Dot(springDir, wheelWorldVel);
            //calculate the magnitude of the damped spring force
            float force = (offset * Strength) - (vel * Damping);
            rb.AddForceAtPosition(springDir * force, wheel.position);

            //Steering
            Vector3 steeringDir = wheel.right;
            //worldspace velocity of the suspension
            //what is the wheels velocity in the steering direction?
            // note that steeringDir is a unit vector, so this returns the magnitude of wheelWorldVel
            float steeringVel = Vector3.Dot(steeringDir, wheelWorldVel);
            // the change in velocity that we're looking for is -steeringVel*gripFactor
            // grip factor is in range 0-1,0 means no grip, 1 means full grip
            float desiredVelChange = -steeringVel * WheelGripFactor;
            //turn change in velocity into an acceleration
            //this will produce the acceleration necessary to change the velocity by desiredVelChange in 1 physics step
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            Debug.Log(steeringDir * WheelMass * desiredAccel);
            rb.AddForceAtPosition(steeringDir * WheelMass * desiredAccel, wheel.position);
        }
    }

    private void FixedUpdate()
    {
        foreach (var wheel in wheels)
        {
            AddSteeringForces(wheel);
        }
    }
}

public class WheelSuspensionSpring
{
    public float Offset;
    public float Strength = 100;
    public float Damping = 10;

    public float GetForce(float velocity)
    {
        return (Offset * Strength) - (velocity * Damping);
    }

}
