using UnityEngine;

public class ClickIndicator : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.3f; // Duration the indicator stays visible
    private SpriteRenderer spriteRenderer;
    private float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false); // Start inactive until configured
    }
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                gameObject.SetActive(false); // Hide the indicator after lifetime expires
            }
        }
    }
    public void Show(Vector3 position, bool isEnemy)
    {
        transform.position = position; // Set the indicator's position
        spriteRenderer.color = isEnemy ? Color.red : Color.green; // Set color based on target type
        gameObject.SetActive(true); // Show the indicator
        timer = lifetime; // Reset the timer
    }
}
