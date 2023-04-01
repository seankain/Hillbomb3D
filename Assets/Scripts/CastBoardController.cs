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
    public float SurfaceMatchSmoothing = 5f;
    public float SmoothDampVelocity = 5f;
    public float PlayerRotateSpeed = 5f;
    public float AirRotateSpeed = 15f;
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
        if (Input.GetAxis("Respawn") > 0)
        {
            Respawn();
        }
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
                //TODO smooth these out, also the weird snapping that happens when the character gets a raycast hit but isnt even close to rotated correctly
                //gameObject.transform.rotation = SmoothDampQuaternion(gameObject.transform.rotation, Quaternion.Euler(hitInfo.normal),ref SmoothDampVelocity,SurfaceMatchSmoothing );
                //gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(hitInfo.normal),SmoothDampVelocity);
                gameObject.transform.rotation *= Quaternion.FromToRotation(gameObject.transform.up, hitInfo.normal);

                gameObject.transform.Rotate(Vector3.down, -horizontal * PlayerRotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            //Todo different spin rates for air versus ground
            gameObject.transform.Rotate(Vector3.down, -horizontal * AirRotateSpeed * Time.deltaTime);
        }


    }

    private static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float velocity, float smoothTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
          Mathf.SmoothDampAngle(c.x, t.x, ref velocity, smoothTime),
          Mathf.SmoothDampAngle(c.y, t.y, ref velocity, smoothTime),
          Mathf.SmoothDampAngle(c.z, t.z, ref velocity, smoothTime)
        );
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
        //Credit to Toyful games from their Very Very Valet tutorial
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

    private void Respawn()
    {
        //Bailed = false;
        //bailElapsed = 0f;
        //characterState.Respawn();
        var respawn = GameObject.FindGameObjectsWithTag("Respawn")[0];
        //characterState.gameObject.transform.parent = characterParent;
        //characterState.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 90, 0));
        //characterState.gameObject.transform.SetPositionAndRotation(characterBoardPosition.position, characterBoardPosition.rotation);
        //PlayerRespawned.Invoke(this, new EventArgs());
        //OnPlayerRespawned(new EventArgs());
        this.transform.position = respawn.transform.position;
        this.transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;


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
