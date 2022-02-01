using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkPassedEventArgs { }

public delegate void ChunkPassedEventHandler(object sender, ChunkPassedEventArgs e);

public class HillChunk : MonoBehaviour
{
    public GameObject ChunkStart;
    public GameObject ChunkEnd;
    public bool IsPositionResetChunk;
    public bool Passed = false;
    public bool Occupied = false;
    public bool IsRespawnChunk = false;

    public ChunkTrigger EntryTrigger;
    public ChunkTrigger ExitTrigger;

    public Waypoint InboundTopWaypoint;
    public Waypoint InboundBottomWaypoint;
    public Waypoint OutboundTopWaypoint;
    public Waypoint OutboundBottomWaypoint;

    public event ChunkPassedEventHandler ChunkPassed;

    [SerializeField]
    private ParkedCar[] parkedCars;

    // Start is called before the first frame update
    void Start()
    {
        EntryTrigger.ChunkEntered += HandleChunkEnter;
        ExitTrigger.ChunkExited += HandleChunkExit;
        parkedCars = GetComponentsInChildren<ParkedCar>();
    }

    public void CycleObstacles()
    {
        foreach(var p in parkedCars)
        {
            p.Cycle();
        }
    }

    protected virtual void OnChunkPassed(ChunkPassedEventArgs e)
    {
        ChunkPassedEventHandler handler = ChunkPassed;
        handler?.Invoke(this, e);
    }


    void HandleChunkEnter(object sender,ChunkTransitEventArgs e) 
    {
        Occupied = true;
    }
    void HandleChunkExit(object sender,ChunkTransitEventArgs e) 
    {
        Passed = true;
        Occupied = false;
        OnChunkPassed(new ChunkPassedEventArgs());

    }

}
