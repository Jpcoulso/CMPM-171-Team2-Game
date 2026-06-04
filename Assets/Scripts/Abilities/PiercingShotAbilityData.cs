using UnityEngine;

// Piercing Shot — fires a long straight line from caster toward cursor.
// Deals heavy damage to ALL enemies along the line.
// Visual: bright white line that fades out quickly.
[CreateAssetMenu(menuName = "RPG/Abilities/PiercingShot")]
public class PiercingShotAbilityData : AbilityData
{
    [Header("Piercing Shot Settings")]
    public float damage = 30f;
    public float lineLength = 12f;
    public float lineWidth = 0.15f;
    public float visualDuration = 0.3f;

    public override void Execute(Character owner)
    {
        // Direction from caster toward cursor
        Vector3 mouseScreen = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector3 origin = owner.transform.position;
        Vector2 direction = ((Vector2)(mouseWorld - origin)).normalized;

        // Raycast along the line and damage all enemies
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, lineLength);
        foreach (RaycastHit2D hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
            }
        }
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Piercing_sfx);

        // Visual: white line from origin along the direction
        Vector3 endPoint = origin + (Vector3)(direction * lineLength);
        CreateLineVisual(origin, endPoint);
    }

    private void CreateLineVisual(Vector3 start, Vector3 end)
    {
        GameObject visual = new GameObject("PiercingShot_Visual");
        visual.transform.position = start;

        LineRenderer lr = visual.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.sortingLayerName = "Characters";
        lr.sortingOrder = 10;

        // Bright white material
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(1f, 1f, 1f, 1f);
        lr.endColor = new Color(0.9f, 0.9f, 1f, 0.7f);

        // Fade out and destroy
        PiercingShotVisual fader = visual.AddComponent<PiercingShotVisual>();
        fader.Init(lr, visualDuration);
    }

    public override void ApplyPassive(Character owner) { }
}
