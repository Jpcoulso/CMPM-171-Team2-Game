using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
     // The one global reference anyone can access
    public static SquadManager Instance { get; private set; }
    private List<Hero> currentSquad = new List<Hero>();
    private const int MAX_SQUAD_SIZE = 4;
    public void ClearSquad() => currentSquad.Clear();

    private void Awake()
    {
        // When this object wakes up, it registers
        // itself as THE instance
        if (Instance != null)
        {
            // One already exists — destroy this duplicate
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Squad details persist
        DontDestroyOnLoad(gameObject);
    }

    public void AddHero(Hero newHero)
    {
        if(currentSquad.Count >= MAX_SQUAD_SIZE)
        {
            return;
        }
        // if squad not full add hero
        currentSquad.Add(newHero);
    }

    public void Removehero(Hero hero)
    {
        currentSquad.Remove(hero);
        if (currentSquad.Count == 0 && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public IReadOnlyList<Hero> GetSquad()
    {
        return currentSquad;
    }
}
