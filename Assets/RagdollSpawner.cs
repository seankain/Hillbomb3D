using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSpawner : MonoBehaviour
{
    public GameObject RagdollPrefab;
    public GameObject Player;

    private GameObject ragdollInstance;

    public GameObject Spawn() 
    {
        ragdollInstance = Instantiate(RagdollPrefab, Player.transform.position, Player.transform.rotation, null);
        return ragdollInstance;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<BoardController>().PlayerRespawned += RagdollSpawner_PlayerRespawned;
    }

    private void RagdollSpawner_PlayerRespawned(object sender, System.EventArgs e)
    {
        if(ragdollInstance != null)
        {
            Destroy(ragdollInstance);
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
