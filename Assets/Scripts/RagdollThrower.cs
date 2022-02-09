using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollThrower : MonoBehaviour
{
    public Vector3 ThrowForce;
    private Rigidbody rb;
    private bool applyForce = false;
    Rigidbody[] rbs;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rbs = GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        applyForce = Input.GetKey(KeyCode.F);
    }

    private void FixedUpdate()
    {

        if (applyForce)
        {
            foreach(var r in rbs)
            {
                r.AddForce(ThrowForce, ForceMode.Impulse);
            }
            //rb.AddForce(ThrowForce, ForceMode.Impulse);
        }
        
    }
}
