using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private BoardControllerBase playerController;

    // Start is called before the first frame update
    void Start()
    {
        FrontPool = new List<HillChunk>();
        PassedPool = new List<HillChunk>();
        FrontPool.AddRange(ChunkPool);
        playerController = Player.GetComponent<BoardControllerBase>();
        playerController.PlayerRespawned += ChunkCycler_PlayerRespawned;
    }

    private void ChunkCycler_PlayerRespawned(object sender, System.EventArgs e)
    {
        ResetChunks();
    }

    /// <summary>
    /// Put the chunks back in an order where the respawn chunk is at the top. Before if the respawn chunk was last there would be no transition chunk in front and they would
    /// start to pop up at the player on chunk exit
    /// </summary>
    public void ResetChunks()
    {
        HillChunk respawnChunk = null;
        List<HillChunk> contentChunks = new List<HillChunk>();
        foreach (var chunk in ChunkPool)
        {
            if (chunk.IsRespawnChunk)
            {
                respawnChunk = chunk;
                var pos = chunk.gameObject.transform.position;
                var distance = Vector3.Distance(pos, Vector3.zero);
                chunk.transform.position -= pos;
                chunk.CycleObstacles();
                chunk.Occupied = true;
            }
            else
            {
                contentChunks.Add(chunk);
            }
        }
        var chunkEndPos = respawnChunk.ChunkEnd.transform.position;
        foreach(var contentChunk in contentChunks)
        {
            MoveChunkToPosition(contentChunk,chunkEndPos);
            chunkEndPos = contentChunk.ChunkEnd.transform.position;
            contentChunk.Occupied = false;
            contentChunk.Passed = false;
        }

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
                MoveObstacles(chunk,pos);
            }
            if (chunk.Passed)
            {
                MoveChunk(chunk);
            }
        }
    }

    public bool TryGetNeighborChunk(HillChunk currentChunk,TravelDirection direction,out HillChunk neighborChunk) 
    {
        var sortedChunks = ChunkPool.OrderBy(c => c.transform.position.z).ToArray();
        for(var i = 0; i < sortedChunks.Length; i++)
        {
            if(sortedChunks[i].name == currentChunk.name)
            {
                if(direction == TravelDirection.Inbound) 
                {
                    //Go forward in z axis since chunks progress in z axis and inbound is positive on z
                    if(i+1<=sortedChunks.Length - 1)
                    {
                        neighborChunk = sortedChunks[i + 1];
                        return true; 
                    }
                }
                else
                {
                    //Go backward in z axis
                    if (i - 1 >= 0)
                    {
                        neighborChunk = sortedChunks[i - 1];
                        return true;
                    }
                }
            }
        }
        neighborChunk = null;
        return false;
    }

    void MoveChunk(HillChunk chunk)
    {
        var minChunkEnd = FindMinChunkEnd();
        var dist = chunk.ChunkStart.transform.position - minChunkEnd;
        chunk.transform.position = (chunk.transform.position - dist);
        //Debug.Log($"Moving {chunk.name} to  {chunk.transform.position}");
        chunk.Passed = false;
        //WaterPlane.transform.position = new Vector3(WaterPlane.transform.position.x, minChunkEnd.y - WaterPlaneOffset, WaterPlane.transform.position.z);
        MoveObstacles(chunk, dist);
        StartCoroutine(LowerWaterPlane());
        chunk.CycleObstacles();
    }

    void MoveChunkToPosition(HillChunk chunk, Vector3 position)
    {
        var dist = chunk.ChunkStart.transform.position - position;
        chunk.transform.position = (chunk.transform.position - dist);
        //Debug.Log($"Moving {chunk.name} to  {chunk.transform.position}");
        chunk.Passed = false;
        //WaterPlane.transform.position = new Vector3(WaterPlane.transform.position.x, minChunkEnd.y - WaterPlaneOffset, WaterPlane.transform.position.z);
        MoveObstacles(chunk, dist);
        StartCoroutine(LowerWaterPlane());
        chunk.CycleObstacles();
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

    private void MoveObstacles(HillChunk chunk, Vector3 dist)
    {
        var pos = chunk.gameObject.transform.position;
        var npcVehicles = FindObjectsOfType<NpcVehicle>();
        foreach(var npcVehicle in npcVehicles)
        {
            if(npcVehicle.CurrentChunk == chunk)
            {
            
                npcVehicle.transform.position = (npcVehicle.transform.position - dist);
            }
        }
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
