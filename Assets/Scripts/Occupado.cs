using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupado : MonoBehaviour
{

    private List<Collider> Occupiers = new List<Collider>();
    public bool Occupied { get { return Occupiers.Count > 0; } }

    private void OnTriggerEnter(Collider other)
    {
        Occupiers.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Occupiers.Remove(other);   
    }

}
