using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheelie : MonoBehaviour
{

    public float MaxSteerAngle = 30f;
    public float TurnSpeed = 10f;
    private WheelCollider wc;

    // Start is called before the first frame update
    void Start()
    {
        wc = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(wc.steerAngle) < MaxSteerAngle)
        {
            wc.steerAngle += Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
        }

        //vertical = Input.GetAxis("Vertical");
    }

}
