using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private int cost;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }
    public void Purchase()
    {
        if (GameManager.Instance.SpendGold(cost))
        {
            bool added = GameManager.Instance.AddHeroToParty(heroData);
            if (added)
            {
                button.interactable = false; // Disable the button after purchase
                Debug.Log("Purchased hero: " + heroData.heroName);
            } else {
                Debug.Log("Party is full, cannot add hero: " + heroData.heroName);
                GameManager.Instance.AddGold(cost);
            }
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
}