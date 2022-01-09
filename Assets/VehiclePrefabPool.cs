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

    private List<VehicleEmitterLocation> GetUnoccupiedEmitterLocations()
    {
        var unoccupied = new List<VehicleEmitterLocation>();
        for(var i = 0; i < vehicleEmitters.Length; i++)
        {
            if (!vehicleEmitters[i].Occupied)
            {
                unoccupied.Add(vehicleEmitters[i]);
            }
        }
        return unoccupied;
    }

    private bool TryGetSpawnPosition(out VehicleEmitterLocation position) 
    {
        var unoccupiedEmitters = GetUnoccupiedEmitterLocations();
        if(unoccupiedEmitters.Count < 1) { position = null; return false; }
        var idx = Random.Range(0, unoccupiedEmitters.Count - 1);
        position = unoccupiedEmitters[idx];
        return true;
    }

    private void SpawnVehicle()
    {
        if (TryGetSpawnPosition(out var spawnPosition))
        {
            if (pool.Count < MaxPoolSize)
            {
                var prefab = Instantiate(VehiclePrefab, spawnPosition.Enter.position, spawnPosition.Enter.rotation, null);
                var vehicle = prefab.GetComponent<NpcVehicle>();
                vehicle.CurrentChunk = spawnPosition.ParentChunk;
                prefab.GetComponent<MeshCycler>().SetRandomMesh();
                pool.Add(vehicle);
            }
            else
            {
                foreach (var p in pool)
                {
                    if (!p.isActiveAndEnabled)
                    {
                        p.gameObject.SetActive(true);
                        p.gameObject.transform.position = spawnPosition.Enter.position;
                        p.gameObject.transform.rotation = spawnPosition.Enter.rotation;
                        p.CurrentChunk = spawnPosition.ParentChunk;
                        p.GetComponent<MeshCycler>().SetRandomMesh();
                    }
                }
            }
        }
    }

}
