using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Party")]
    [SerializeField] private HeroData[] partyData = new HeroData[4];
    // To create a starting character
    [SerializeField] private HeroData startingHero;

    [Header("Spawning")]
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private Transform[] heroSpawns;

    [Header("Dev Tools")]
    [SerializeField] public int currentGold = 0;

    // Stores health of heroes
    private float[] savedHealth;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        // To create a starting character
        if (partyData[0] == null && startingHero != null)
        {
            partyData[0] = startingHero;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeCombat());
    }
    private IEnumerator InitializeCombat()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;
        SpawnHeroes();
        
        yield return null; // wait another frame to ensure heroes are spawned
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.StartNextWave();
        } else
        {
            Debug.LogWarning("WaveManager not found in scene!");
        }
    }
    private void SpawnHeroes()
    {
        if (heroPrefab == null || heroSpawns == null || heroSpawns.Length == 0)
        {
            Debug.LogError("Hero prefab or spawn points not set!");
            return;
        }
        int spawnCount = Mathf.Min(partyData.Length, heroSpawns.Length);
        for (int i = 0; i < spawnCount; i++)
        {
            if (partyData[i] != null)
            {
                if (partyData[i] == null) continue; // skip empty slots
                Transform spawnPoint = heroSpawns[i];
                GameObject heroGO = Instantiate(heroPrefab, spawnPoint.position, Quaternion.identity);
                Hero hero = heroGO.GetComponent<Hero>();
                if (hero != null)
                {
                        // If we have saved HP for this slot, pass it in; otherwise full health (0 = use max)
                    float savedHP = (savedHealth != null && i < savedHealth.Length) ? savedHealth[i] : 0f;
                    hero.Init(partyData[i], savedHP);
                } else
                {
                    Debug.LogError("Hero prefab missing Hero component!");
                }
            }
        }
    }

    public bool AddHeroToParty(HeroData data)
    {
        for (int i = 0; i < partyData.Length; i++)
        {
            if (partyData[i] == null)
            {
                partyData[i] = data;
                return true; // success
            }
        }
        return false; // party full
    }

    public void RemoveHeroFromParty(int slot)
    {
        if (slot < 0 || slot >= partyData.Length) return;
        partyData[slot] = null;
        if (savedHealth != null && slot < savedHealth.Length)
        {
            savedHealth[slot] = 0f; // clear their saved HP too
        }
    }

    // Resource handling
    public void AddGold(int amount)
    {
        currentGold += amount;
    }
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            return true;
        }
        return false; // not enough gold
    }
}

