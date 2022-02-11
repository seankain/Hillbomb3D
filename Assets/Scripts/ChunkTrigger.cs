using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChunkTriggerType
{
    Entry,
    Exit
}

public class ChunkTransitEventArgs { }

public delegate void ChunkEnteredEventHandler(object sender, ChunkTransitEventArgs e);
public delegate void ChunkExitedEventHandler(object sender, ChunkTransitEventArgs e);


public class ChunkTrigger : MonoBehaviour
{

    public event ChunkEnteredEventHandler ChunkEntered;
    public event ChunkExitedEventHandler ChunkExited;

    protected virtual void OnChunkEntered(ChunkTransitEventArgs e)
    {
        ChunkEnteredEventHandler handler = ChunkEntered;
        handler?.Invoke(this, e);
    }

    protected virtual void OnChunkExited(ChunkTransitEventArgs e)
    {
        ChunkExitedEventHandler handler = ChunkExited;
        handler?.Invoke(this, e);
    }

    public ChunkTriggerType TriggerType;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && TriggerType == ChunkTriggerType.Entry)
        {
            if (!other.GetComponent<BoardControllerBase>().Bailed)
            {
                OnChunkEntered(new ChunkTransitEventArgs());
            }
            //Debug.Log($"Player entering chunk");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && TriggerType == ChunkTriggerType.Exit)
        {
            if (!other.GetComponent<BoardControllerBase>().Bailed)
            {
                OnChunkExited(new ChunkTransitEventArgs());
            }
            //Debug.Log($"Player exiting chunk");
        }
    }
}
