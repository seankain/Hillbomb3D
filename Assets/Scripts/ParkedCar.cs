using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkedCar : MonoBehaviour
{

    private MeshCycler meshCycler;
    private Occupado occupado;

    public void Cycle()
    {
        Debug.Log($"Occupado {occupado.Occupied}");
        if(Random.value > 0.5 && occupado.Occupied == false)
        {
            meshCycler.SetRandomMesh();
        }
        else
        {
            meshCycler.TurnOffAllMeshes();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        meshCycler = GetComponent<MeshCycler>();
        occupado = GetComponent<Occupado>();
        Cycle();
    }
}
