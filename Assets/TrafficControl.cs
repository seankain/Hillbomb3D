using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficControl : MonoBehaviour
{

    public TrafficLight[] TrafficLights;
    public TrafficLight[] OppositeTrafficLights;
    public float CycleChangeTime = 30f;
    private float elapsed = 0;

    private bool state = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= CycleChangeTime)
        {
            elapsed = 0;
            FlipLights();
        }
    }

    private void FlipLights()
    {
        if (state)
        {
            foreach(var t in TrafficLights)
            {
                t.SetLightStop();
            }
            foreach(var o in OppositeTrafficLights)
            {
                o.SetLightGo();
            }
        }
        else
        {
            foreach (var t in TrafficLights)
            {
                t.SetLightGo();
            }
            foreach (var o in OppositeTrafficLights)
            {
                o.SetLightStop();
            }
        }
        state = !state;
    }
}
