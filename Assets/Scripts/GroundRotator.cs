using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRotator : MonoBehaviour
{
    public float Speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R)) {
            this.transform.Rotate(Vector3.right, Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.T))
        {
            this.transform.Rotate(Vector3.right, -Speed * Time.deltaTime);
        }
    }
}
