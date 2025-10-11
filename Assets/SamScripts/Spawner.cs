using UnityEngine;
using System.Collections;

public class RepeatedPrefabSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject prefabToSpawn;      // Prefab to spawn
    public float spawnDelay = 2f;         // Time between spawns
    public bool startOnAwake = true;      // Start spawning automatically

    private void Start()
    {
        if (startOnAwake)
            StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (prefabToSpawn != null)
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
