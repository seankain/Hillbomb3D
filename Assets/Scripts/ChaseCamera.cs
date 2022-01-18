using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{

    public GameObject ChaseObject;
    public float ChaseDistance = 10f;
    public Vector3 ChaseOffset;
    public Vector3 ChaseOffsetRotation;
    public float ChaseHeight = 10f;
    public float ChaseSpeed = 4f;
    public float Smoothing = 0.125f;
    private Rigidbody chaseRigidBody;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        chaseRigidBody = ChaseObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var velocity = chaseRigidBody.velocity;
        //cam.transform.LookAt(ChaseObject.transform);
        //cam.transform.position = ChaseObject.transform.position + ChaseOffset;
        Vector3 desiredPosition = ChaseObject.transform.position + ChaseOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(gameObject.transform.position, desiredPosition,ref velocity,Smoothing * Time.deltaTime, 10f);
        cam.transform.position = desiredPosition;
        //if (Vector3.Distance(cam.transform.position, ChaseObject.transform.position) > ChaseDistance)
        //{
        //cam.transform.position = Vector3.MoveTowards(cam.transform.position, ChaseObject.transform.position, ChaseSpeed * Time.deltaTime);

        //}
    }
}