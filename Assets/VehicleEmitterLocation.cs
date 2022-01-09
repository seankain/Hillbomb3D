using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEmitterLocation : MonoBehaviour
{

    public VehicleEmitterDirection Direction;
    public bool Occupied { get { return occupiers.Count > 1; } }
    public Transform Enter;
    public Transform Exit;
    public HillChunk ParentChunk;
    private List<Collider> occupiers;

    // Start is called before the first frame update
    void Start()
    {
        if (ParentChunk == null)
        {
            ParentChunk = gameObject.GetComponentInParent<HillChunk>();
        }
        occupiers = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        occupiers.Add(other);
    }
    public void OnTriggerExit(Collider other)
    {
        occupiers.Remove(other);
    }
}

public enum VehicleEmitterDirection
{
    Source,
    Sink
}
