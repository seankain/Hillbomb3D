using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEmitterLocation : MonoBehaviour
{

    public VehicleEmitterDirection Direction;
    public Transform Enter;
    public Transform Exit;
    public HillChunk ParentChunk;

    // Start is called before the first frame update
    void Start()
    {
        if (ParentChunk == null)
        {
            ParentChunk = gameObject.GetComponentInParent<HillChunk>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Direction == VehicleEmitterDirection.Sink)
        {

        }
    }
}

public enum VehicleEmitterDirection
{
    Source,
    Sink
}
