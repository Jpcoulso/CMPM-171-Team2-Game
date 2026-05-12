using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Gold, }

    [SerializeField] private PickupType type;
    [SerializeField] public int goldAmount;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case PickupType.Gold:
                Debug.Log($"Picked up {goldAmount} gold!");
                break;
        }

        Destroy(gameObject);
    }
}