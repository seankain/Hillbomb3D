using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderConfigWatcher : MonoBehaviour, IObservable<WheelColliderConfig>
{
    public IDisposable Subscribe(IObserver<WheelColliderConfig> observer)
    {
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}