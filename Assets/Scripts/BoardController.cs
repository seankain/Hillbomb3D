using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public bool Grounded = false;
    public Vector3 LevelDirection = Vector3.forward;
    public Vector3 SurfaceAngleEuler = Vector3.zero;
    public float ArbitraryConstant =1;
    private Vector3 OriginalPosition;
    private Vector3 SteerDirection = Vector3.forward;
    public float TurnForceConstant = 1;

    private Rigidbody rb;

    const float radConvert = Mathf.PI / 180f;
    const float downhillAcc = (2f / 3f) * 9.81f;
    private float horizontal = 0;
    private float vertical = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        OriginalPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetAxis("Jump") > 0) { Respawn(); }
    }

    private void FixedUpdate()
    {
        var acc = ArbitraryConstant * (LevelDirection * (2f / 3f) * 9.8f * Mathf.Sin(radConvert * SurfaceAngleEuler.x));
        var turnForce = TurnForceConstant * Vector3.right * horizontal;
        //Debug.Log(acc);
        //Debug.Log(turnForce);
        //Debug.Log(Vector3.Cross(acc, turnForce));
        rb.AddForce(turnForce,ForceMode.VelocityChange);
        rb.AddForce(acc);
    }

    private void OnCollisionStay(Collision collision)
    {
        Grounded = collision.gameObject.tag == "RidingSurface";
        //var contacts = new ContactPoint[collision.contactCount];
        //collision.GetContacts(contacts);
        SurfaceAngleEuler = collision.gameObject.transform.rotation.eulerAngles;
        
        
    }

    private void Respawn()
    {
        this.transform.position = OriginalPosition;
        this.transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }



}
