using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCharacterController : MonoBehaviour
{
    const float radConvert = Mathf.PI / 180f;
    public float TurnSpeed = 0.01f;
    public float Acceleration = 5f;
    public float CurrentVelocity = 0f;
    public float RayGroundDistance = 1f;
    private Vector3 SurfaceAngleEuler = Vector3.zero;
    private CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down,Color.red);
        if (Physics.Raycast(ray, out var hitInfo, 1000f, ~(1 << LayerMask.NameToLayer("Skateboard"))))
        {
            //Debug.Log($"{hitInfo.collider.gameObject.name} {hitInfo.normal}");
            SurfaceAngleEuler = hitInfo.normal;
            if (hitInfo.distance < RayGroundDistance)
            {
                gameObject.transform.position = hitInfo.point + new Vector3(0, RayGroundDistance, 0);

                //if (characterController.isGrounded)
                //{
                gameObject.transform.rotation *= Quaternion.FromToRotation(gameObject.transform.up, hitInfo.normal);
            }
            //}
        }
        var acc = (Vector3.forward * (2f / 3f) * 9.81f * Mathf.Sin(radConvert * SurfaceAngleEuler.x));
        // var turnForce = (TurnForceConstant * rb.velocity.magnitude + turnTime) * Vector3.right * horizontal;
        var turnForce = Vector3.right * horizontal * TurnSpeed * Time.deltaTime;
        var downForce = Vector3.down * 9.81f * Time.deltaTime;
        gameObject.transform.Translate(acc);
        //gameObject.transform.Translate(turnForce);
        //gameObject.transform.Translate(downForce);

        //characterController.Move(Vector3.forward * Acceleration * Time.deltaTime);
        //if (characterController.isGrounded)
        //{
        //    Debug.Log(Vector3.right * horizontal * Time.deltaTime);
        //   characterController.Move(turnForce);
        //}
        //else
        //{

        //    characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
        //}

    }
}
