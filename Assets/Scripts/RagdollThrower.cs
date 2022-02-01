using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollThrower : MonoBehaviour
{
    public Vector3 ThrowForce;
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
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(ThrowForce, ForceMode.Impulse);
        }
    }
}
