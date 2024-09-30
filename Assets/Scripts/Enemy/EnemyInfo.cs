using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyInfos", menuName = "Scriptable Objects/EnemyInfos", order = 1)]
public class EnemyInfo : ScriptableObject
{
    public GameObject EnemyObject { get; set; }
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float maxDistance; // 초기위치로부터 이동할 수 있는 최대 거리

    [SerializeField] private float chaseSpeed;

    [SerializeField] private int level;
    [SerializeField] private float maxHealth;

    [SerializeField] private int normalAttackDamage;
    [SerializeField] private int strongAttackDamage;
    [SerializeField] private float timeBetweenAttacks;

    // getter
    public float SightRange => sightRange;
    public float AttackRange => attackRange;
    public float MaxDistance => maxDistance;
    public float ChaseSpeed => chaseSpeed;
    public int Level => level;
    public float MaxHealth => maxHealth;
    public int NormalAttackDamage => normalAttackDamage;
    public int StrongAttackDamage => strongAttackDamage;
    public float TimeBetweenAttacks => timeBetweenAttacks;

}
