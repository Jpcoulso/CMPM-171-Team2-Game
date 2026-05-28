using UnityEngine;

// Temporary slow effect — reduces movement speed, shows blue tint, cleans up on expire.
public class SlowEffect : MonoBehaviour
{
    private Character target;
    private float slowAmount;
    private float duration;
    private float timer;
    private SpriteRenderer sr;
    private Color originalColor;

    public void Init(Character target, float slowAmount, float duration)
    {
        this.target = target;
        this.slowAmount = slowAmount;
        this.duration = duration;
        timer = 0f;

        // Apply slow
        target.moveSpeedMultiplier = slowAmount;

        // Blue tint to show slowed
        sr = target.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
            sr.color = new Color(0.5f, 0.6f, 1f, originalColor.a); // icy blue tint
        }
    }

    public void Refresh(float newDuration)
    {
        timer = 0f;
        duration = newDuration;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Expire();
        }
    }

    private void Expire()
    {
        if (target != null)
            target.moveSpeedMultiplier = 1f;
        if (sr != null)
            sr.color = originalColor;
        Destroy(this);
    }

    private void OnDestroy()
    {
        // Safety cleanup if enemy dies while slowed
        if (target != null && target.moveSpeedMultiplier == slowAmount)
            target.moveSpeedMultiplier = 1f;
        if (sr != null)
            sr.color = originalColor;
    }
}
