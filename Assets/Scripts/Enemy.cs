using UnityEngine;
// Enemy.cs
// Goes on your Goblin GameObject.
// Drag Goblin.asset into the enemyData field.

public class Enemy : Character
{
    [SerializeField] private EnemyData enemyData;

    public override float MaxHealth    => enemyData.maxHealth;
    public override float AttackDamage => enemyData.attackDamage;
    public override float MoveSpeed    => enemyData.moveSpeed;
    public override bool IsRanged      => enemyData.isRanged; // might be obselete

    public override float AttackRange => enemyData.attackRange;

    public override string GetCharacterName() => enemyData.enemyName;


    // ─────────────────────────────────────────
    // MOVEMENT
    // ─────────────────────────────────────────
    protected override void MoveTowards(Vector3 position)
    {
        
    }
    protected override void MoveToDestination(){}
    protected override void FaceTarget(Vector3 position){}
    protected override bool HasReachedDestination(){return false;}





    private void Start()
    {
        currentHealth = MaxHealth;
        Debug.Log($"{GetCharacterName()} spawned! HP: {currentHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log($"{GetCharacterName()} defeated! " +
                  $"Rewarding {enemyData.experienceReward} XP.");

        //WaveManager.Instance.OnEnemyDied(this); --- uncomment when wavemanager is implemented
        //gameObject.SetActive(false);
    }
}
