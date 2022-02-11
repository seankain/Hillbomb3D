using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoller : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rb;
    public Collider playerCollider;
    // Start is called before the first frame update

    private void Awake()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetRagdoll(bool ragdollActivated)
    {
        var colliders = GetComponentsInChildren<Collider>(true);
        playerCollider.enabled = !ragdollActivated;
        anim.enabled = !ragdollActivated;
        rb.velocity = Vector3.zero;
        rb.isKinematic = ragdollActivated;
        foreach (var collider in colliders)
        {
            if (collider != playerCollider)
            {
                collider.isTrigger = !ragdollActivated;
                collider.attachedRigidbody.isKinematic = !ragdollActivated;
                collider.attachedRigidbody.velocity = Vector3.zero;
            }
        }
    }

  
}
