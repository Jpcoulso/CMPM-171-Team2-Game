using System.Collections.Generic;
using UnityEngine;

public class TelegraphAbility : MonoBehaviour
{
    public enum Shape { Circle, Line, Cone }

    [Header("Ability")]
    [SerializeField] private Shape shape = Shape.Circle;
    [Tooltip("Seconds between casts.")]
    [SerializeField] private float castCooldown = 6f;
    [Tooltip("Charge-up time before the AOE resolves — the player's dodge window.")]
    [SerializeField] private float castWindup = 1.75f;
    [SerializeField] private float abilityDamage = 30f;
    [Tooltip("Freeze the enemy in place while charging the cast.")]
    [SerializeField] private bool freezeWhileCasting = true;
    [Tooltip("Wait this long after spawn before the first cast can happen.")]
    [SerializeField] private float initialDelay = 2f;

    [Header("Circle (puddle)")]
    [SerializeField] private float circleRadius = 2.5f;

    [Header("Line (rectangle)")]
    [SerializeField] private float lineLength = 7f;
    [SerializeField] private float lineWidth = 2f;

    [Header("Cone (fan)")]
    [SerializeField] private float coneRadius = 5f;
    [SerializeField] private float coneAngle = 75f;

    private Character character;
    private float cooldownTimer;
    private float lockTimer;
    private bool casting;

    public void SetShape(Shape s) => shape = s;

    private void Awake()
    {

        character = GetComponent<Character>();
        if (character == null) character = GetComponentInParent<Character>();
        if (character == null) character = GetComponentInChildren<Character>(true);
    }

    private void Start()
    {
        cooldownTimer = Mathf.Max(initialDelay, 0f);
    }

    private void Update()
    {
        if (character == null || character.IsDead) return;

        if (casting)
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
            {
                casting = false;
                if (freezeWhileCasting) character.isKnockedBack = false;
            }
            return;
        }

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return;

        Vector3 targetPos;
        if (!TryGetTarget(out targetPos)) return;

        BeginCast(targetPos);
    }

    private bool TryGetTarget(out Vector3 targetPos)
    {
        targetPos = default;

        Character target = character.Target;
        if (target != null && !target.IsDead && InAttackRange(target.transform.position))
        {
            targetPos = target.transform.position;
            return true;
        }

        if (SquadManager.Instance == null) return false;

        IReadOnlyList<Hero> squad = SquadManager.Instance.GetSquad();
        Hero closest = null;
        float closestDist = float.MaxValue;
        foreach (Hero hero in squad)
        {
            if (hero == null || hero.IsDead) continue;
            if (!InAttackRange(hero.transform.position)) continue;

            float d = Vector2.Distance(character.transform.position, hero.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                closest = hero;
            }
        }

        if (closest == null) return false;
        targetPos = closest.transform.position;
        return true;
    }
    private bool InAttackRange(Vector3 heroPos)
    {
        Vector3 pos = character.transform.position;
        if (character.IsRanged)
            return Vector2.Distance(pos, heroPos) <= character.AttackRange;

        return Mathf.Abs(pos.y - heroPos.y) <= 0.2f
            && Mathf.Abs(pos.x - heroPos.x) <= character.AttackRange;
    }

    private void BeginCast(Vector3 targetPosition)
    {
        cooldownTimer = castCooldown;
        SpawnTelegraph(targetPosition);

        if (freezeWhileCasting)
        {
            casting = true;
            lockTimer = castWindup;
            character.isKnockedBack = true;
        }
    }

    private void SpawnTelegraph(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;

        GameObject go = new GameObject($"{name}_Telegraph");
        AoeTelegraph t = go.AddComponent<AoeTelegraph>();

        switch (shape)
        {
            case Shape.Line:
            {
                Vector2 dir = (Vector2)(targetPosition - origin);
                go.transform.position = origin;
                t.InitRectangle(lineLength, lineWidth, dir, castWindup, abilityDamage, character);
                break;
            }
            case Shape.Cone:
            {
                Vector2 dir = (Vector2)(targetPosition - origin);
                go.transform.position = origin;
                t.InitCone(coneRadius, coneAngle, dir, castWindup, abilityDamage, character);
                break;
            }
            case Shape.Circle:
            default:
                go.transform.position = targetPosition;
                t.InitCircle(circleRadius, castWindup, abilityDamage, character);
                break;
        }
    }
}
