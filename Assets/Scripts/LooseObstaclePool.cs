using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LooseObstaclePool : MonoBehaviour
{
    public GameObject[] ObstaclePrefabs;
    public float SpawnDelay = 5f;
    public float SpawnAheadDistance = 30f;
    private float elapsed = 0;
    private GameObject player;
    private List<GameObject> obstacles = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= SpawnDelay)
        {
            SpawnObstacle();
            StartCoroutine(CleanupOldObstacles());
            elapsed = 0;
        }
    }

    private void SpawnObstacle()
    {
        var pos = player.transform.position + Vector3.forward * SpawnAheadDistance;
        Instantiate(ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Length)], pos, Quaternion.identity, null);
    }

    private IEnumerator CleanupOldObstacles()
    {
        for(var i = obstacles.Count - 1; i >= 0; i--)
        {
            if(obstacles[i] == null)
            {
                obstacles.RemoveAt(i);
            }
            else
            {
                if(obstacles[i].transform.position.z < player.transform.position.z)
                {
                    Destroy(obstacles[i]);
                    obstacles.RemoveAt(i);
                }
            }
            yield return null;
        }
    }
}
