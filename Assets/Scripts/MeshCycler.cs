using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshCycler : MonoBehaviour
{
    [SerializeField]
    public BodyMeshInfo[] Meshes;

    [SerializeField]
    public Material[] BodyMaterials;

    [SerializeField]
    public ChassisConfiguration Chassis;
    public BodyMeshInfo ActiveBodyMesh { get; private set; }

    public void SetActiveMesh(int index)
    {
        if(index < 0 || index > Meshes.Length - 1)
        {
            Debug.LogWarning("Trying to access mesh renderer index outside of available range. Ignoring.");
            return;
        }
        ActiveBodyMesh = Meshes[index];
        for(var i = 0; i < Meshes.Length; i++)
        {
            Meshes[i].BodyMesh.SetActive(i == index);
        }
        UpdateChassisConfiguration(Meshes[index]);
        SetRandomBodyMaterial();
    }

    public void SetRandomMesh()
    {
        SetActiveMesh(Random.Range(0, Meshes.Length));
    }

    private void UpdateChassisConfiguration(BodyMeshInfo info) 
    {
        Chassis.LeftFront.transform.position = info.BodyMesh.transform.Find("LeftFront").position;
        Chassis.LeftRear.transform.position = info.BodyMesh.transform.Find("LeftRear").position;
        Chassis.RightRear.transform.position = info.BodyMesh.transform.Find("RightRear").position;
        Chassis.RightFront.transform.position = info.BodyMesh.transform.Find("RightFront").position;
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


[Serializable]
public class ChassisConfiguration
{
    public GameObject LeftFront;
    public GameObject RightFront;
    public GameObject LeftRear;
    public GameObject RightRear;
    public Rigidbody VehicleRigidBody;
}

[Serializable]
public class BodyMeshInfo
{
    public GameObject BodyMesh;
    public int SwapMaterialPostition = 0;
}
