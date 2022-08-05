using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedWobbleModel : MonoBehaviour
{
    public float Velocity = 1;
    //kg
    public float RiderMass = 75;
    //kg
    public float BoardMass = 3;
    //alpha distance between trucks meters
    public float Wheelbase = 0.6f;
    //height between axle and deck
    public float Height = 0.07f;
    //lambda sub f and lambda sub r for front and rear, is the angle in degrees between the baseplateand the kingpin
    public float BushingPivotAngleRear = 45;
    public float BushingPivotAngleFront = 45;
    //kappa sub gamma sqrt Nmrad^-1
    public float TorsionalSpringStiffness = 250;
    //kilogram meters squared
    public float MomentOfInertiaX = 0.025f;
    //gamma board tilt angle, the input by rider influenced by ankle position
    public float TiltAngle = 0;
    //delta sub f
    public float FrontSteeringAngle;
    //delta sub r
    public float RearSteringAngle;
    //Assuming equal torsional spring stiffness between front and rear truck
    public float RestoringTorque { get { return TorsionalSpringStiffness * TiltAngle; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
