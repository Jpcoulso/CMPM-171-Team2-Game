using UnityEngine;

// Fades out the piercing shot line then destroys it.
public class PiercingShotVisual : MonoBehaviour
{
    private LineRenderer lr;
    private float duration;
    private float timer;

    public void Init(LineRenderer lineRenderer, float duration)
    {
        lr = lineRenderer;
        this.duration = duration;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Fade alpha over time
        float alpha = 1f - t;
        lr.startColor = new Color(1f, 1f, 1f, alpha);
        lr.endColor = new Color(0.9f, 0.9f, 1f, alpha * 0.7f);
    }
}
