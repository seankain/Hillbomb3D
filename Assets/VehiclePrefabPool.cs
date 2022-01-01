using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePrefabPool : MonoBehaviour
{

    public Transform SpawnPosition;
    public GameObject VehiclePrefab;
    public int MaxPoolSize = 10;
    public float SpawnDelay = 1f;
    private float elapsed = 0;
    private List<NpcVehicle> pool;

    // Start is called before the first frame update
    void Start()
    {
        pool = new List<NpcVehicle>();
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

    void SpawnVehicle()
    {
        if (pool.Count < MaxPoolSize)
        {
            var prefab = Instantiate(VehiclePrefab, SpawnPosition.position, SpawnPosition.rotation, null);
            var vehicle = prefab.GetComponent<NpcVehicle>();
            prefab.GetComponent<MeshCycler>().SetRandomMesh();
            pool.Add(vehicle);
        }
    }

}
