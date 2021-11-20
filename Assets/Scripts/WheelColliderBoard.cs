using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderBoard : MonoBehaviour
{

    private WheelCollider[] wcs;
    private float RearRotation = 0;
    private float FrontRotation = 0;
    private float vertical;
    public float PushForce = 1f;
    public float Torque = 1f;
    public GameObject FrontAxel;
    public GameObject RearAxel;
    public WheelCollider LeftFrontWheel;
    public WheelCollider RightFrontWheel;
    public WheelCollider LeftRearWheel;
    public WheelCollider RightRearWheel;
    public float MaxTurnDegrees = 30f;
    public float TurnSpeed = 4;
    private Vector3 OriginalPosition;
   

    // Start is called before the first frame update
    void Start()
    {
        wcs = GetComponentsInChildren<WheelCollider>();
        OriginalPosition = this.transform.position;
        //rb = GetComponent<Rigidbody>();
        //foreach(var wc in wcs)
        //{
        //    wc.motorTorque = Torque;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        TurnAxels(Input.GetAxis("Horizontal"));
        vertical = Input.GetAxis("Vertical");
        if(Input.GetAxis("Jump") > 0) { Respawn(); }

    }

    private void TurnAxels(float turn)
    {
        if (turn == 0)
        {
            FrontRotation -= (TurnSpeed * Time.deltaTime);
            RearRotation -= (TurnSpeed * Time.deltaTime);
            if(FrontRotation < 0)
            {
                FrontRotation = 0;
                RearRotation = 0;
            }
        }
        else
        {
            FrontRotation += turn * (TurnSpeed * Time.deltaTime);
            RearRotation += turn * (TurnSpeed * Time.deltaTime);

            if (FrontRotation > MaxTurnDegrees)
            {
                FrontRotation = MaxTurnDegrees;
                RearRotation = MaxTurnDegrees;
            }
            if (FrontRotation < -MaxTurnDegrees)
            {
                FrontRotation = -MaxTurnDegrees;
                RearRotation = -MaxTurnDegrees;
            }
        }
    }

    private void FixedUpdate()
    {
        //foreach (var wc in wcs)
        //{
        //    wc.motorTorque = vertical * Torque;
        //}
        LeftFrontWheel.steerAngle = FrontRotation;
        LeftFrontWheel.motorTorque = vertical * Torque;
        RightFrontWheel.steerAngle = FrontRotation;
        RightFrontWheel.motorTorque = vertical * Torque;
        LeftRearWheel.steerAngle = RearRotation;
        LeftRearWheel.motorTorque = vertical * Torque;
        RightRearWheel.steerAngle = RearRotation;
        RightRearWheel.motorTorque = vertical * Torque;
        //if (vertical != 0)
        //{
        //    Debug.Log(vertical);
        //    rb.AddForce(Vector3.forward * PushForce * Time.deltaTime);
        //}
    }

    private void Respawn()
    {
        this.transform.position = OriginalPosition;
        this.transform.rotation = Quaternion.identity;
    }

}
