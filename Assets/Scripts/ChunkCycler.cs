using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCycler : MonoBehaviour
{
    public float ChunkLength = 100;
    public float HillAngle = 17;
    public float WaterPlaneOffset = 30;
    public GameObject WaterPlane;
    public List<HillChunk> ChunkPool;
    private List<HillChunk> FrontPool;
    private List<HillChunk> PassedPool;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        FrontPool = new List<HillChunk>();
        PassedPool = new List<HillChunk>();
        FrontPool.AddRange(ChunkPool);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var chunk in ChunkPool)
        {
            if (chunk.Occupied && chunk.IsPositionResetChunk)
            {
                //Going to move all of the chunks back to around zero to avoid floating point issues from sustained play
                var pos = chunk.gameObject.transform.position;
                var distance = Vector3.Distance(pos, Vector3.zero);
                foreach (var c in ChunkPool)
                {
                    c.transform.position -= pos;
                }
                Player.transform.position -= pos;
            }
            if (chunk.Passed)
            {
                MoveChunk(chunk);
            }
        }
    }

    void MoveChunk(HillChunk chunk)
    {
        var minChunkEnd = FindMinChunkEnd();
        var dist = chunk.ChunkStart.transform.position - minChunkEnd;
        chunk.transform.position = (chunk.transform.position - dist);
        //Debug.Log($"Moving {chunk.name} to  {chunk.transform.position}");
        chunk.Passed = false;
        //WaterPlane.transform.position = new Vector3(WaterPlane.transform.position.x, minChunkEnd.y - WaterPlaneOffset, WaterPlane.transform.position.z);
        StartCoroutine(LowerWaterPlane());
    }

    Vector3 FindMinChunkEnd()
    {
        var min = Vector3.zero;
        HillChunk minChunk = null;
        foreach (var c in ChunkPool)
        {
            if (c.ChunkEnd.transform.position.y < min.y && c.ChunkEnd.transform.position.z > min.z)
            {
                min = c.ChunkEnd.transform.position;
                minChunk = c;
            }
        }
        //Debug.Log($"Min chunk is {minChunk.gameObject.name} at {min}");
        return min;
    }

    IEnumerator LowerWaterPlane()
    {
        var min = FindMinChunkEnd() - new Vector3(0, WaterPlaneOffset, 0);
        while (WaterPlane.transform.position.y > min.y - WaterPlaneOffset)
        {
            WaterPlane.transform.position = Vector3.MoveTowards(WaterPlane.transform.position, min, Time.deltaTime * 5f);
            yield return null;
        }
    }

}
