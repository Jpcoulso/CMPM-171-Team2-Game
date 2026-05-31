using UnityEngine;
// EnemyData.cs
[CreateAssetMenu(fileName = "NewEnemy", menuName = "RPG/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public bool isRanged;

    [Header("Base Stats")]
    public float maxHealth;
    public float attackDamage;
    public float attackRange;
    public float attackRate;
    public float moveSpeed;
    public float detectionRange;
    public int experienceReward;
    public int goldReward;

}
