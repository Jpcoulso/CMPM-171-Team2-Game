using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ClickIndicator : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.3f; // Duration the indicator stays visible
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        Destroy(gameObject, lifetime); // Automatically destroy after lifetime expires
    }
    public void SetColor(bool isEnemy)
    {
        spriteRenderer.color = isEnemy ? Color.red : Color.green; // Red for enemies, green for allies
    }
}
