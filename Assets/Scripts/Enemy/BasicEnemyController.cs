using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyController : MonoBehaviour, IEnemyController
{
    [SerializeField] EnemyInfo enemyInfo;
    Animator animator;
    EnemyCombatComponent combatComponent;

    private NavMeshAgent agent;
    Vector3 initialLocation;
    Vector3 lastLocation;
    [SerializeField] float walkPointRange;
    private Vector3 walkPoint;
    private bool walkPointSet = false;

    [SerializeField] float patrolSpeed;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyInfo.EnemyObject = gameObject;
        enemyInfo.EnemyTransform = transform;
        combatComponent = new BasicEnemyCombatComponent()
        {
            OwnerController = this,
            Agent = agent,
            EnemyInfo = enemyInfo
        };
        InvokeRepeating(nameof(IsMoving), 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!combatComponent.PlayerInSightRange && !combatComponent.PlayerInAttackRange) 
            Patrolling();
    }

     void Patrolling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        agent.speed = patrolSpeed;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

            animator.SetBool("Walk", walkPointSet);

         if (name.Contains("Slime") || name.Contains("Turtle") || name.Contains("Mushroom"))
        {
            if (combatComponent.targetFound)//발견했었지만 시야범위 벗어났을 때
            {
                combatComponent.targetFound = false;
                animator.SetBool("See", false);
            }

            if(name.Contains("Mushroom")) return;

            //walkPoint에 도달했으면, 슬라임/거북이만 실행
            if (!walkPointSet)
            {
                animator.SetBool("Stop", true);
            }
            else
            {
                animator.SetBool("Stop", false);
            }
        }
    }

    void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // 초기 위치 범위 내인지 체크
        if (Vector3.Distance(transform.position, initialLocation) > enemyInfo.MaxDistance)
        {
            walkPoint = initialLocation;
        }

        walkPointSet = true;
        lastLocation = transform.position;
    }

    void IsMoving()
    {
        if (combatComponent.PlayerInSightRange || combatComponent.PlayerInAttackRange) return;

        if(Vector3.Distance(lastLocation, transform.position) < 0.5f) // 이동하지 않았음
        {
            walkPointSet = false;
            Debug.Log("이동 지점 재설정");
        }
    }

    public void TakeDamage(int damage)
    {
        combatComponent.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return combatComponent.IsDead;
    }

    public float GetMaxHealth()
    {
        return enemyInfo.MaxHealth;
    }
}
