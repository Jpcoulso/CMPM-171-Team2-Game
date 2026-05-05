using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    [SerializeField] private Transform[] spawnPoints; // Assign in inspector
    [SerializeField] private WaveData[] waves; // Define waves in inspector
    [System.Serializable] public class WaveData
    {
        public GameObject[] enemies;
    }
    private int enemiesRemaining;
    private int currentWave = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartNextWave();
    }
    public void StartNextWave()
    {
        if (currentWave >= waves.Length)
        {
            Debug.Log("All waves completed!");
            return;
        }
        
        WaveData wave = waves[currentWave];
        enemiesRemaining = wave.enemies.Length;
        currentWave++;
        
        foreach (GameObject enemyPrefab in wave.enemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
        
    }

    public void OnEnemyDied(Enemy enemy)
    {
        // Handle enemy death, e.g., check if wave is cleared, reward player, etc.
        Debug.Log($"Enemy {enemy.GetCharacterName()} has died. Handling wave logic...");
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            WaveCleared();
        }
    }
    public void WaveCleared()
    {
        Debug.Log("Wave cleared!");
        StartNextWave();
    }
}