using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Enemy Setup")]
    public GameObject enemyPrefab;    // drag enemy prefab (AIBotController + EnemyAttack + EnemyHealth)
    public Transform[] spawnPoints;   // drag spawn points here

    [Header("Levels")]
    public int startingEnemies = 2;   // Level 1
    public int enemiesPerLevel = 1;   // adds each level

    int currentLevel = 1;
    List<GameObject> aliveEnemies = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartLevel(currentLevel);
    }

    void StartLevel(int level)
    {
        Debug.Log("Starting level " + level);
        aliveEnemies.Clear();

        int enemyCount = startingEnemies + (level - 1) * enemiesPerLevel;

        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawn = spawnPoints[i % spawnPoints.Length];
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
            aliveEnemies.Add(enemy);
        }
    }

    public void OnEnemyDied(GameObject enemy)
    {
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);

        if (aliveEnemies.Count == 0)
        {
            currentLevel++;
            StartLevel(currentLevel);
        }
    }
}
