using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePrefabPool : MonoBehaviour
{

    //public Transform SpawnPosition;
    public GameObject VehiclePrefab;
    public int MaxPoolSize = 10;
    public float SpawnDelay = 1f;
    private float elapsed = 0;
    private List<NpcVehicle> pool;
    private ChunkCycler chunkCycler;
    private VehicleEmitterLocation[] vehicleEmitters;

    // Start is called before the first frame update
    void Start()
    {
        pool = new List<NpcVehicle>();
        chunkCycler = FindObjectOfType<ChunkCycler>();
        vehicleEmitters = FindObjectsOfType<VehicleEmitterLocation>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= SpawnDelay)
        {
            elapsed = 0;
            SpawnVehicle();
        }
    }

    private VehicleEmitterLocation GetSpawnPosition() 
    {
        var idx = Random.Range(0, vehicleEmitters.Length - 1);
        return vehicleEmitters[idx];
        //foreach(var c in chunkCycler.ChunkPool)
        //{
        //    gameObject.transform.w
        //}
    }

    private void SpawnVehicle()
    {
        var spawnPosition = GetSpawnPosition();
        if (pool.Count < MaxPoolSize)
        {
            var prefab = Instantiate(VehiclePrefab, spawnPosition.Enter.position, spawnPosition.Enter.rotation, null);
            var vehicle = prefab.GetComponent<NpcVehicle>();
            prefab.GetComponent<MeshCycler>().SetRandomMesh();
            pool.Add(vehicle);
        }
    }

}
