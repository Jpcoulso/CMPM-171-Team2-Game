using UnityEngine;

// Fades out a volley arrow line then destroys it.
public class VolleyArrowVisual : MonoBehaviour
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

        float alpha = 1f - t;
        lr.startColor = new Color(1f, 1f, 1f, alpha * 0.9f);
        lr.endColor = new Color(0.85f, 0.85f, 0.9f, alpha * 0.5f);
    }
}
