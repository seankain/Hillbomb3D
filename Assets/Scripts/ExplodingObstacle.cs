using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingObstacle : MonoBehaviour
{

    public float SplodeyForce = 10000;
    public float Upwards = 10000;
    public float SplodeyRadius = 5f;
    public GameObject ExplosionPrefab;
    public AudioSource ExplosionAudio;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, null);
            var rb = collision.collider.gameObject.GetComponent<Rigidbody>();
            rb.AddExplosionForce(SplodeyForce, gameObject.transform.position, SplodeyRadius,Upwards);
            collision.collider.gameObject.GetComponentInChildren<BoardController>().Bail();

            ExplosionAudio.Play();

            //Destroy(gameObject);
        }
    }
}
