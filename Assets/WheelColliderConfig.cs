using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderConfig : MonoBehaviour
{


    /// <summary>
    /// The colliders in which to overwrite the values on start
    /// </summary>
    public List<WheelCollider> OverwrittenColliders;

    /// <summary>
    /// The mass of the wheels
    /// </summary>
    public float Mass;
    private float mass;

    /// <summary>
    /// The radius of the wheels
    /// </summary>
    public float Radius;

    private float radius;
    //private float radius;

    /// <summary>
    /// This is a value of damping applied to a wheel.
    /// </summary>
    public float WheelDampingRate;

    /// <summary>
    /// Maximum extension distance of wheel suspension, measured in local space.
    /// Suspension always extends downwards through the local Y-axis.
    /// </summary>
    public float SuspensionDistance;

    /// <summary>
    /// This parameter defines the point where the wheel forces will applied.
    /// This is expected to be in metres from the base of the wheel at rest position along the suspension travel direction.
    /// When forceAppPointDistance = 0 the forces will be applied at the wheel base at rest.
    /// A better vehicle would have forces applied slightly below the vehicle centre of mass.
    /// </summary>
    public float ForceAppPointDistance;

    /// <summary>
    /// Center of the wheel in object local space.
    /// </summary>
    public Vector3 Center;

    /// <summary>
    /// The suspension attempts to reach a Target Position by adding spring and damping forces.
    /// Spring force attempts to reach the Target Position. A larger value makes the suspension reach the Target Position faster.
    /// Damper 	Dampens the suspension velocity. A larger value makes the Suspension Spring move slower.
    /// Target Position  The suspension’s rest distance along Suspension Distance. 1 maps to fully extended suspension,
    /// and 0 maps to fully compressed suspension.Default value is 0.5, which matches the behavior of a regular car’s suspension.
    /// </summary>
    /// Wont serialize to editor window even though it does for the wheel collider script itself
    public JointSpring SuspensionSpring { get; set; }

    //
    // Summary:
    //     The spring forces used to reach the target position.
    public float SuspensionSpringSpring;
    //
    // Summary:
    //     The damper force uses to dampen the spring.
    [field: SerializeField]
    public float SuspensionSpringDamper;
    //
    // Summary:
    //     The target position the joint attempts to reach.
    public float SuspensionSpringTargetPosition;

    /// <summary>
    /// Extremum Slip/Value Curve’s extremum point.
    /// Asymptote Slip/Value Curve’s asymptote point.
    /// Stiffness    Multiplier for the Extremum Value and Asymptote Value(default is 1).
    /// Changes the stiffness of the friction.Setting this to zero will completely disable all friction from the wheel.
    /// Usually you modify stiffness at runtime to simulate various ground materials from scripting.
    /// </summary>
    [SerializeField]
    public WheelFrictionCurve ForwardFriction;
    public float ForwardFrictionExtremumSlip=1;
    public float ForwardFrictionAsymptoteSlip=2;
    public float ForwardFrictionExtremumValue=20000;
    public float ForwardFrictionAsymptoteValue=10000;
    public float ForwardFrictionStiffness = 1;

    /// <summary>
    /// See Forward friction
    /// </summary>
    [SerializeField]
    public WheelFrictionCurve SidewaysFriction;
    public float SideFrictionExtremumSlip =1;
    public float SideFrictionAsymptoteSlip = 2;
    public float SideFrictionExtremumValue = 20000;
    public float SideFrictionAsymptoteValue = 10000;
    public float SideFrictionStiffness = 1;

    private void ResizeRadius(float value)
    {
        Debug.Log("Resizing Radius");
        foreach(var wc in OverwrittenColliders)
        {
            wc.radius = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //All of the wheel colliders should be the same in this case
        var wc = OverwrittenColliders[0];
        radius = wc.radius;
        mass = wc.mass;

       

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(radius != Radius)
        //{
            radius = Radius;
            foreach (var wc in OverwrittenColliders)
            {
                wc.radius = radius;
                wc.mass = Mass;
                //wc.sidewaysFriction.
                wc.wheelDampingRate = WheelDampingRate;
                wc.suspensionDistance = SuspensionDistance;
                wc.suspensionSpring = new JointSpring { damper = SuspensionSpringDamper, spring = SuspensionSpringSpring, targetPosition = SuspensionSpringTargetPosition };
                wc.center = Center;
                wc.forceAppPointDistance = ForceAppPointDistance;
                wc.forwardFriction = new WheelFrictionCurve { asymptoteSlip = ForwardFrictionAsymptoteSlip, extremumSlip = ForwardFrictionExtremumSlip, stiffness = ForwardFrictionStiffness, asymptoteValue = ForwardFrictionAsymptoteValue, extremumValue = ForwardFrictionExtremumValue };
                wc.sidewaysFriction = new WheelFrictionCurve { asymptoteSlip = SideFrictionAsymptoteSlip, extremumSlip = SideFrictionExtremumSlip, stiffness = SideFrictionStiffness };
            }
        //}
        //if(mass != Mass)
        //{
        //    mass = Mass;
        //    foreach(var wc in OverwrittenColliders)
        //    {
        //        wc.mass = mass;
        //    }
        //}


    }
}


