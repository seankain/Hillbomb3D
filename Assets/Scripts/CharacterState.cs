using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{

    public Animator anim;
    public Rigidbody rb;
    public Transform FrontBoltPosition;
    public Transform RearBoltPosition;

    public Vector3 FrontBoltVec;
    public Vector3 RearBoltVec;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        anim.SetBool("Pushing", Input.GetAxis("Vertical") > 0); 
        anim.SetFloat("Speed", rb.velocity.magnitude);
    }

    public void Ollie()
    {
        anim.SetTrigger("Ollie");
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!anim.GetBool("Pushing"))
        {
            FrontBoltVec = FrontBoltPosition.position;
            RearBoltVec = RearBoltPosition.position;
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, FrontBoltPosition.position);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, FrontBoltPosition.transform.rotation);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, RearBoltPosition.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, RearBoltPosition.transform.rotation);
        }

    }

}
