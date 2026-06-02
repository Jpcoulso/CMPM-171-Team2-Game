using UnityEngine;
using System.Collections;
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
    private int spawnIndex = 0;

    private void Awake()
    {
        Instance = this;
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
            // Spawns on spawnpoints in sequence, wrapping back on overflow
            Transform spawnPoint = spawnPoints[spawnIndex % spawnPoints.Length];
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnIndex++;
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
        if (currentWave >= waves.Length)
        {
            Debug.Log("All waves completed!");
            // Later: trigger end-of-level rewards, transition to next scene, etc.
            // Add time padding to allow players to collect resources
            StartCoroutine(LoadArmoryAfterDelay());
            return;
            
        }

        ScreenText.Instance.ShowCountdown($"Wave {currentWave + 1} in", 3, StartNextWave);
    }
    IEnumerator LoadArmoryAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 2 seconds delay
        GameManager.Instance.LoadScene("Map");
    }
    IEnumerator LoadNextWaveAfterDelay()
    {
        // Could add some UI feedback here, e.g., "Next wave in 3...2...1..."
        yield return new WaitForSeconds(2f); // 2 seconds delay
        StartNextWave();
    }
}