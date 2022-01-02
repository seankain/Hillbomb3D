using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{

    public Transform GreenLightPosition;
    public Transform YellowLightPosition;
    public Transform RedLightPosition;
    public Light LightEmitter;
    public Color GreenLightColor;
    public Color YellowLightColor;
    public Color RedLightColor;
    public TrafficLightState CurrentState;

    // Start is called before the first frame update
    void Start()
    {
        SetLightStop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetLightGo()
    {
        LightEmitter.color = GreenLightColor;
        LightEmitter.transform.position = GreenLightPosition.position;
        CurrentState = TrafficLightState.LightGo;
    }
    public void SetLightStop() 
    {
        LightEmitter.color = RedLightColor;
        LightEmitter.transform.position = RedLightPosition.position;
        CurrentState = TrafficLightState.LightStop;
    }
    public void SetLightSlow() 
    {
        LightEmitter.color = YellowLightColor;
        LightEmitter.transform.position = YellowLightPosition.position;
        CurrentState = TrafficLightState.LightSlow;
    }
}

public enum TrafficLightState
{
    LightStop,
    LightSlow,
    LightGo
}