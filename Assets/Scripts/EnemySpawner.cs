using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab; // Assign the enemy prefab
    [SerializeField] private Transform[] spawnPoints; // Array of spawn points
    [SerializeField] private int enemiesPerWave = 5; // Number of enemies per wave
    [SerializeField] private float timeBetweenWaves = 10f; // Time between waves
    [SerializeField] private float spawnDelay = 0.5f; // Delay between spawns

    private Queue<GameObject> enemyPool; // The pool of enemy objects
    private int currentWave = 0;

    void Start()
    {
        // Initialize the enemy pool
        InitializePool();

        // Start spawning waves
        StartCoroutine(SpawnWaves());
    }

    private void InitializePool()
    {
        enemyPool = new Queue<GameObject>();

        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false); // Deactivate initially
            enemyPool.Enqueue(enemy);
        }
    }

    private System.Collections.IEnumerator SpawnWaves()
    {
        while (true)
        {
            currentWave++;
            Debug.Log($"Wave {currentWave} starting!");

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay); // Delay between spawns
            }

            Debug.Log($"Wave {currentWave} complete! Waiting for the next wave...");
            yield return new WaitForSeconds(timeBetweenWaves); // Wait for next wave
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            Debug.LogWarning("Spawn points or enemy prefab not set!");
            return;
        }

        if (enemyPool.Count == 0)
        {
            Debug.LogWarning("Enemy pool is empty. Consider increasing the pool size.");
            return;
        }

        // Get an enemy from the pool
        GameObject enemy = enemyPool.Dequeue();

        // Reset the enemy
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.ResetEnemy();
        }

        // Activate and position the enemy
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = spawnPoint.rotation;
        enemy.SetActive(true);

        Debug.Log($"Enemy spawned at {spawnPoint.position} for Wave {currentWave}.");
    }





    public void ReturnToPool(GameObject enemy)
    {
        // Deactivate and return the enemy to the pool
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
