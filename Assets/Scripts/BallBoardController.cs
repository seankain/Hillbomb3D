using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBoardController : MonoBehaviour
{
    public float TurnSpeed = 1f;
    [SerializeField]
    private GameObject Visual;
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
        rb.AddForce(Vector3.right * Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime,ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision)
    {
        for(var i = 0; i < collision.contactCount; i++)
        {
            if(collision.GetContact(i).otherCollider.gameObject.layer == LayerMask.NameToLayer("RidingSurface"))
            {
                var fromTo = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);
                var rot = Quaternion.Euler(fromTo.x, 0, fromTo.y);
                Visual.transform.rotation *= rot;
            }
        }

    }
}
