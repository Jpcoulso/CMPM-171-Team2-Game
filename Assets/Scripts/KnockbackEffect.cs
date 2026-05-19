using UnityEngine;

// KnockbackEffect.cs
// Attach this at runtime to smoothly launch a character in a direction.
// It pauses the character's state machine during the knockback, then cleans itself up.

public class KnockbackEffect : MonoBehaviour
{
    private Rigidbody2D rb;
    private Character character;
    private Vector2 velocity;
    private float duration;
    private float timer;

    public void Initialize(Vector2 direction, float distance, float knockbackDuration)
    {
        rb = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();

        duration = knockbackDuration;
        timer = 0f;

        // Calculate initial velocity so the character decelerates to a stop over the duration
        // Using simple linear deceleration: distance = velocity * duration / 2
        // So velocity = 2 * distance / duration
        velocity = direction.normalized * (2f * distance / duration);

        // Pause the state machine
        if (character != null)
        {
            character.isKnockedBack = true;
        }
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer >= duration)
        {
            // Knockback finished — resume normal behaviour
            if (character != null)
            {
                character.isKnockedBack = false;
                character.ClearTarget();
            }
            Destroy(this); // remove just the component, not the GameObject
            return;
        }

        // Lerp velocity down to zero for a smooth deceleration
        float t = timer / duration;
        Vector2 currentVelocity = Vector2.Lerp(velocity, Vector2.zero, t);
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}
