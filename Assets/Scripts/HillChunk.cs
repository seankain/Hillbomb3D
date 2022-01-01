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

    public ChunkTrigger EntryTrigger;
    public ChunkTrigger ExitTrigger;

    public event ChunkPassedEventHandler ChunkPassed;

    // Start is called before the first frame update
    void Start()
    {
        EntryTrigger.ChunkEntered += HandleChunkEnter;
        ExitTrigger.ChunkExited += HandleChunkExit;
    }

    // Update is called once per frame
    void Update()
    {
        
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
