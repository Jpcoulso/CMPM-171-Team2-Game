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
    private Transform[] heroSpawns;

    [Header("Levels")]
    public bool[] levelUnlocked;

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
        // Levels
        levelUnlocked = new bool[5];
        levelUnlocked[0] = true;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HeroSpawnPoint[] spawns = FindObjectsByType<HeroSpawnPoint>(FindObjectsSortMode.None);
        if (spawns == null || spawns.Length == 0)
        {
            Debug.LogWarning("No hero spawn points found in scene!");
            return;
        }
        heroSpawns = new Transform[spawns.Length];
        for (int i = 0; i < spawns.Length; i++)
        {
            heroSpawns[i] = spawns[i].transform;
        }
        if (heroSpawns.Length > 0) { StartCoroutine(InitializeCombat()); }
    }
    // Used for buttons to access
    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName, false);
    }
    // Overload for when we want to carry over health values
    public void LoadScene(string sceneName, bool carryOverHealth = false)
    {
        if (carryOverHealth)
        {
            var squad = SquadManager.Instance.GetSquad();
            savedHealth = new float[squad.Count];
            for (int i = 0; i < squad.Count; i++)
            {
                savedHealth[i] = squad[i].CurrentHealth;
            }
        }
        else { savedHealth = null; }
        SceneManager.LoadScene(sceneName);
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
        if (heroSpawns == null || heroSpawns.Length == 0)
        {
            Debug.LogError("No hero spawn points found!");
            return;
        }
        int spawnCount = Mathf.Min(partyData.Length, heroSpawns.Length);
        for (int i = 0; i < spawnCount; i++)
        {
            if (partyData[i] == null) continue; // skip existing heroes in party
            if (partyData[i].heroPrefab == null)
            {
                Debug.LogError($"Hero {partyData[i].heroName} has no prefab assigned!");
                continue;
            }
            GameObject heroGO = Instantiate(partyData[i].heroPrefab, heroSpawns[i].position, Quaternion.identity);
            Hero hero = heroGO.GetComponent<Hero>();
            if (hero != null)
            {
                float savedHP = (savedHealth != null && i < savedHealth.Length) ? savedHealth[i] : partyData[i].maxHealth;
                hero.Init(partyData[i], savedHP);
            }else {
                Debug.LogError($"Hero prefab for {partyData[i].heroName} does not have a Hero component!");
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
    // Levels
    public void UnlockLevel(int level)
    {
        if (level >= 0 && level < levelUnlocked.Length)
            levelUnlocked[level] = true;
    }
    public bool IsLevelUnlocked(int level)
    {
        if (level >= 0 && level < levelUnlocked.Length)
            return levelUnlocked[level];

        return false;
    }
}

