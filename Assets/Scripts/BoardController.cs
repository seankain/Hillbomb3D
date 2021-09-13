using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public bool Grounded = false;
    public Vector3 LevelDirection = Vector3.forward;
    public Vector3 SurfaceAngleEuler = Vector3.zero;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.Cross(Vector3.down * 9.8f * Time.deltaTime,LevelDirection * SurfaceAngleEuler.magnitude), ForceMode.Force);
    }

    private void OnCollisionStay(Collision collision)
    {
        Grounded = collision.gameObject.tag == "RidingSurface";
        //var contacts = new ContactPoint[collision.contactCount];
        //collision.GetContacts(contacts);
        SurfaceAngleEuler = collision.gameObject.transform.rotation.eulerAngles;
        
        
    }

    


}
