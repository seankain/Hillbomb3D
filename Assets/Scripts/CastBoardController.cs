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
    public bool SnapToSurface = true;
    public float UprightStability = 0.3f;
    public float UprightSpeed = 2.0f;
    public bool UprightSingleAxis = false;
    private Transform[] wheels;
    private WheelInfo[] wheelInfos;
    public float SurfaceMatchSmoothing = 1f;
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
    private float vertical = 0f;
    private float currentTurnAngle = 0f;
    private bool Grounded = false;

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
        wheelInfos = new WheelInfo[4]
        {
            new WheelInfo(LeftFrontWheel),
            new WheelInfo(RightFrontWheel),
            new WheelInfo(LeftRearWheel),
            new WheelInfo(RightRearWheel)
        };
        // Turning the colliders on messed with the inertia tensors and center of mass so manually setting them
        // until i can figure out a way to do it correctly
        rb.inertiaTensor = new Vector3(11.78971f, 12.80615f, 1.080781f);
        rb.inertiaTensorRotation = Quaternion.Euler(0, 0, 359.9538f);
        rb.centerOfMass = new Vector3(0, -0.0900267f, 0.03401519f);
        //GetComponent<BoxCollider>().enabled = true;
        //GetComponent<CapsuleCollider>().enabled = true;
        //rb.ResetInertiaTensor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Respawn") > 0)
        {
            Respawn();
        }
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        //if(currentTurnAngle < MaxTurnAngle)
        //{
        //TODO fix
        currentTurnAngle += horizontal * TurnSpeed * Time.deltaTime;
        if(horizontal == 0)
        {
            currentTurnAngle -= TurnSpeed * Time.deltaTime;
            if(!Mathf.Approximately(currentTurnAngle, 0))
            {
                currentTurnAngle = 0;
            }
        }
        currentTurnAngle = Mathf.Clamp(currentTurnAngle, -MaxTurnAngle, MaxTurnAngle);
        Debug.Log(Vector3.Dot(transform.forward, Vector3.up));
        if (Vector3.Dot(transform.forward,Vector3.up) <= 0)
        {
            FrontAxle.localEulerAngles = new Vector3(0, currentTurnAngle, 0);
            RearAxle.localEulerAngles = new Vector3(0, -currentTurnAngle, 0);
        }
        else
        {
            FrontAxle.localEulerAngles = new Vector3(0, -currentTurnAngle, 0);
            RearAxle.localEulerAngles = new Vector3(0, currentTurnAngle, 0);
        }
        //}

        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down, Color.yellow);

        if (SnapToSurface)
        {
            foreach (var wheelInfo in wheelInfos)
            {
                if (wheelInfo.Hitting)
                {
                    transform.rotation = Quaternion.FromToRotation(wheelInfo.WheelTransform.up, wheelInfo.SurfaceNormal) * transform.rotation;
                }
                //transform.rotation = Quaternion.LookRotation(wheelInfo.HitWorldLocation - wheelInfo.WheelTransform.position) * _childRPos;
            }
        }

        if (Physics.Raycast(ray, out var hitInfo, 1, ~(1 << LayerMask.NameToLayer("Skateboard"))))
        {
            if (hitInfo.distance < RayGroundDistance)
            {
                Grounded = true;
                if (SnapToSurface)
                {
                    //    //TODO smooth these out, also the weird snapping that happens when the character gets a raycast hit but isnt even close to rotated correctly
                    //    //gameObject.transform.rotation = SmoothDampQuaternion(gameObject.transform.rotation, Quaternion.Euler(hitInfo.normal),ref SmoothDampVelocity,SurfaceMatchSmoothing );
                    //    //gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(hitInfo.normal),SmoothDampVelocity);
                    //    //-------------------
                    //    //var n = new Vector3(hitInfo.normal.x, transform.rotation.eulerAngles.y, hitInfo.normal.z);
                    //    //gameObject.transform.rotation *= Quaternion.FromToRotation(gameObject.transform.up, n);
                    //    //gameObject.transform.Rotate(Vector3.down, -horizontal * PlayerRotateSpeed * Time.deltaTime);
                    //    //This is finnicky still and has some problems compared to the single raycast from the middle
                    //    foreach (var wheelInfo in wheelInfos)
                    //    {
                    //        if (wheelInfo.Hitting)
                    //        {
                    //            transform.rotation = Quaternion.FromToRotation(wheelInfo.WheelTransform.up, wheelInfo.SurfaceNormal) * transform.rotation;
                    //        }
                    //        //transform.rotation = Quaternion.LookRotation(wheelInfo.HitWorldLocation - wheelInfo.WheelTransform.position) * _childRPos;
                    //    }

                    //This works for the single central raycast but its too jerky and unresponsive to small terrain changes or slopes
                    //uncomment the aligning with surface normal to use
                    float headingDeltaAngle = Input.GetAxis("Horizontal") * Time.deltaTime * TurnSpeed;
                    Quaternion headingDelta = Quaternion.AngleAxis(headingDeltaAngle, transform.up);
                    //align with surface normal
                    //transform.rotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
                    //apply heading rotation
                    transform.rotation = headingDelta * transform.rotation;
                }
            }

        }
        else
        {
            Grounded = false;
            //rb.AddTorque(-transform.up * (-horizontal * AirRotateSpeed * Time.deltaTime));
            //gameObject.transform.Rotate(Vector3.down, -horizontal * AirRotateSpeed * Time.deltaTime);
            //gameObject.transform.Rotate(Vector3.right, vertical * AirRotateSpeed * Time.deltaTime);
           //rb.AddTorque(transform.right * (vertical * AirRotateSpeed * Time.deltaTime));
        }


    }


    private static Quaternion SmoothSlerp(Transform current, Vector3 goalPosition,float speed)
    {
        var direction = (goalPosition - current.position).normalized;
        var goal = Quaternion.LookRotation(direction);
        return Quaternion.Slerp(current.rotation,goal,speed);
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

    private void AddSteeringForces(WheelInfo wheelInfo)
    {
        var wheel = wheelInfo.WheelTransform;
        //Credit to Toyful games from their Very Very Valet tutorial
        var ray = new Ray(wheel.position, Vector3.down * CastDistance);
        Debug.DrawRay(wheel.position, Vector3.down * CastDistance, Color.red);
        if (Physics.Raycast(ray, out var hitInfo, CastDistance, ~(1 << LayerMask.NameToLayer("Skateboard"))))
        {
            wheelInfo.SurfaceNormal = hitInfo.normal;
            wheelInfo.Hitting = true;
            wheelInfo.HitWorldLocation = hitInfo.point;
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
            rb.AddForceAtPosition(steeringDir * WheelMass * desiredAccel, wheel.position);
        }
        else
        {
            wheelInfo.Hitting = false;
        }
    }

    private void Respawn()
    {
        //Bailed = false;
        //bailElapsed = 0f;
        //characterState.Respawn();
        var respawn = GameObject.FindGameObjectsWithTag("Respawn")[0];
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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
        foreach (var wheelInfo in wheelInfos)
        {
            AddSteeringForces(wheelInfo);
        }
        if (!Grounded)
        {
            rb.AddTorque(-transform.up * (-horizontal * AirRotateSpeed * Time.fixedDeltaTime));
            rb.AddTorque(transform.right * (vertical * AirRotateSpeed * Time.fixedDeltaTime));
        }
        //trying to find a way to stop wacky flipping without locking rigidbody
        //https://answers.unity.com/questions/10425/how-to-stabilize-angular-motion-alignment-of-hover.html
        var predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * UprightStability / UprightSpeed, rb.angularVelocity) * transform.up;
        var torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        if (UprightSingleAxis)
        {
            Vector3.Project(torqueVector,transform.forward);
        }
        rb.AddTorque(torqueVector * UprightSpeed * UprightSpeed);
        //foreach (var wheel in wheels)
        //{
        //    AddSteeringForces(wheel);
        //}
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

public class WheelInfo
{
    public WheelInfo(Transform wheelTransform)
    {
        this.WheelTransform = wheelTransform;
    }
    public Transform WheelTransform;
    public bool Hitting = false;
    public Vector3 SurfaceNormal;
    public Vector3 HitWorldLocation;
}
