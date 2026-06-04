using UnityEngine;

// Volley — fires a fan of arrows in a cone toward the cursor.
// Each arrow damages and slows enemies it hits.
[CreateAssetMenu(menuName = "RPG/Abilities/Volley")]
public class VolleyAbilityData : AbilityData
{
    [Header("Volley Settings")]
    public int arrowCount = 7;
    public float coneAngle = 50f; // total spread in degrees
    public float arrowLength = 8f;
    public float damage = 8f;
    public float slowAmount = 0.4f; // 60% slow
    public float slowDuration = 3f;
    public float visualDuration = 0.25f;

    public override void Execute(Character owner)
    {
        Vector3 mouseScreen = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector3 origin = owner.transform.position;
        Vector2 centerDir = ((Vector2)(mouseWorld - origin)).normalized;
        float centerAngle = Mathf.Atan2(centerDir.y, centerDir.x) * Mathf.Rad2Deg;

        // Fan arrows evenly across the cone
        float halfCone = coneAngle * 0.5f;
        float step = (arrowCount > 1) ? coneAngle / (arrowCount - 1) : 0f;

        // Track which enemies we already hit so each takes damage only once
        var alreadyHit = new System.Collections.Generic.HashSet<Enemy>();

        for (int i = 0; i < arrowCount; i++)
        {
            float angle = centerAngle - halfCone + (step * i);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // Raycast this arrow
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, arrowLength);
            foreach (RaycastHit2D hit in hits)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead && !alreadyHit.Contains(enemy))
                {
                    alreadyHit.Add(enemy);
                    enemy.TakeDamage(damage);

                    // Apply slow — refresh if already slowed
                    SlowEffect existing = enemy.GetComponent<SlowEffect>();
                    if (existing != null)
                        existing.Refresh(slowDuration);
                    else
                    {
                        SlowEffect slow = enemy.gameObject.AddComponent<SlowEffect>();
                        slow.Init(enemy, slowAmount, slowDuration);
                    }
                }
            }
            AudioManager.Instance.PlaySFX(AudioManager.SFXType.Volley_sfx);

            // Arrow visual
            Vector3 endPoint = origin + (Vector3)(dir * arrowLength);
            CreateArrowVisual(origin, endPoint);
        }
    }

    private void CreateArrowVisual(Vector3 start, Vector3 end)
    {
        GameObject visual = new GameObject("Volley_Arrow");
        visual.transform.position = start;

        LineRenderer lr = visual.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.06f;
        lr.endWidth = 0.03f;
        lr.sortingLayerName = "Heroes";
        lr.sortingOrder = 10;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(1f, 1f, 1f, 0.9f);
        lr.endColor = new Color(0.85f, 0.85f, 0.9f, 0.5f);

        // Fade and destroy
        VolleyArrowVisual fader = visual.AddComponent<VolleyArrowVisual>();
        fader.Init(lr, visualDuration);
    }

    public override void ApplyPassive(Character owner) { }
}
