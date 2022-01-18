using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleCycler : MonoBehaviour
{
    [SerializeField]
    public BodyMeshInfo[] Meshes;

    [SerializeField]
    public Material[] BodyMaterials;
    public BodyMeshInfo ActiveBodyMesh { get; private set; }

    private Occupado occupado;

    [SerializeField]
    private bool Mobile = false;

    private void Start()
    {
        occupado = GetComponent<Occupado>();
        Cycle();
    }

    public void Cycle()
    {
        //Might make an ICycler interface or base class which is why this pointless function exists as is
        if (Random.value > 0.5 && occupado.Occupied == false)
        {
            SetRandomMesh();
        }
        else
        {
            TurnOffAllMeshes();
        }
    }
    public void SetActiveMesh(int index)
    {
        if (index < 0 || index > Meshes.Length - 1)
        {
            Debug.LogWarning("Trying to access mesh renderer index outside of available range. Ignoring.");
            return;
        }
        ActiveBodyMesh = Meshes[index];
        for (var i = 0; i < Meshes.Length; i++)
        {
            Meshes[i].BodyMesh.SetActive(i == index);
        }
        if (BodyMaterials.Length > 1)
        {
            SetRandomBodyMaterial();
        }
    }

    public void SetRandomMesh()
    {
        SetActiveMesh(Random.Range(0, Meshes.Length));
    }

    public void TurnOffAllMeshes()
    {
        for (var i = 0; i < Meshes.Length; i++)
        {
            Meshes[i].BodyMesh.SetActive(false);
        }
    }


    private void SetRandomBodyMaterial()
    {
        var mat = BodyMaterials[Random.Range(0, BodyMaterials.Length)];
        var materials = ActiveBodyMesh.BodyMesh.GetComponent<MeshRenderer>().materials;
        materials[ActiveBodyMesh.SwapMaterialPostition] = mat;
        ActiveBodyMesh.BodyMesh.GetComponent<MeshRenderer>().materials = materials;
        //ActiveBodyMesh.BodyMesh.GetComponent<MeshRenderer>().materials[ActiveBodyMesh.SwapMaterialPostition] = mat;
        Debug.Log(ActiveBodyMesh.BodyMesh.GetComponent<MeshRenderer>().materials[ActiveBodyMesh.SwapMaterialPostition]);
    }

}