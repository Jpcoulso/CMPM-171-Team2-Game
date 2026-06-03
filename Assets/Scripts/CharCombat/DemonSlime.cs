using UnityEngine;
 
public class DemonSlime : Enemy
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointReachedDistance = 0.2f;
    [SerializeField] private float minWanderDistance = 2f;
    [SerializeField] private float maxWanderDistance = 6f;
 
    [Header("Boss Spawn")]
    [SerializeField] private GameObject demonBossPrefab;
 
    private Vector2 currentWaypoint;

 
    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------
 
    protected override void Start()
    {
        base.Start();
        PickNewWaypoint();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isWalking", true);
    }
 
    private void Update()
    {
        if (isDead) return;
 
        MoveTowardsWaypoint();
 
        if (Vector2.Distance(transform.position, currentWaypoint) < waypointReachedDistance)
            PickNewWaypoint();
    }
 
    // -------------------------------------------------------------------------
    // Movement
    // -------------------------------------------------------------------------
 
    private void MoveTowardsWaypoint()
    {
        
        Vector2 direction = (currentWaypoint - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
 
        // Flip sprite to face direction of travel
        if (direction.x != 0)
            transform.localScale = new Vector3(direction.x > 0 ? 1f : -1f, 1f, 1f);
    }
 
    private void PickNewWaypoint()
    {
         Bounds bounds = GetCameraBounds();
 
        float randomDistance = Random.Range(minWanderDistance, maxWanderDistance);
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 candidate = (Vector2)transform.position + randomDirection * randomDistance;
 
        // Clamp the waypoint so the slime never walks off screen
        currentWaypoint = new Vector2(
            Mathf.Clamp(candidate.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(candidate.y, bounds.min.y, bounds.max.y)
        );
    }

    private Bounds GetCameraBounds()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width  = height * cam.aspect;
 
        return new Bounds(
            cam.transform.position,
            new Vector3(width * 2f, height * 2f, 0f)
        );
    }

    // -------------------------------------------------------------------------
    // Death
    // -------------------------------------------------------------------------
 
    protected override void OnDeath()
    {
        base.OnDeath();
        SpawnBoss();
        Destroy(gameObject);
    }
 
    private void SpawnBoss()
    {
        if (demonBossPrefab == null)
        {
            Debug.LogWarning($"{name}: demonBossPrefab is not assigned!");
            return;
        }
 
        Instantiate(demonBossPrefab, transform.position, Quaternion.identity);
    }
}