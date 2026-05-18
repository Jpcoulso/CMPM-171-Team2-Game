using UnityEngine;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private int cost;

    public void Purchase()
    {
        if (GameManager.Instance.SpendGold(cost))
        {
            bool added = GameManager.Instance.AddHeroToParty(heroData);
            if (!added)
                Debug.Log("Party full!");
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
}