using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("General Settings")] // 언리얼의 카테고리와 같은 기능
    [SerializeField] bool isElite = false; // 보스 여부
    Animator animator;
    [SerializeField] LayerMask whatIsPlayer;
    Transform player;

    bool targetFound = false;
    bool isInCombat = false;

    [Header("Health Settings")]
    [SerializeField] float maxHealth;
    float currentHealth;
    bool isDead = false;

    [Header("Movement Settings")]
    Vector3 initialLocation;
    Vector3 lastLocation;
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] float walkPointRange;
    private Vector3 walkPoint;
    private bool walkPointSet = false;

    bool playerInSightRange = false;
    bool playerInAttackRange = false;

    [SerializeField] float patrolSpeed; // 일반 몬스터만 해당
    [SerializeField] float chaseSpeed;
    [SerializeField] float maxDistance; // 초기위치로부터 이동할 수 있는 최대 거리


    [Header("Attack Settings")]
    [SerializeField] float timeBetweenAttacks;
    private bool canAttack = true;

    [Header("Components")]
    private NavMeshAgent agent;

    [Header("Gizmos Settings")]
    public Transform groundCheck;
    public LayerMask groundMask;
    private bool isGrounded;
    private Vector3 velocity;

    // Additional settings for Boss
    public GameObject fireball; // 보스의 특별 공격용 오브젝트

    private void Awake()
    {
        player = GameObject.Find("Fox").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        initialLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        InvokeRepeating(nameof(IsMoving), 1f, 1f);
    }

    void Update()
    {
        // 시야 및 공격 범위 체크
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (currentHealth > 0)
        {
            if (!playerInSightRange && !playerInAttackRange && !isElite) // 일반 몬스터만 순찰
                Patrolling();
            if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();
            if (playerInSightRange && playerInAttackRange)
                AttackPlayer();
        }
        else
        {
            Die();
        }
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
            if (targetFound)//발견했었지만 시야범위 벗어났을 때
            {
                targetFound = false;
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
        if (Vector3.Distance(transform.position, initialLocation) > maxDistance)
        {
            walkPoint = initialLocation;
        }

        walkPointSet = true;
        lastLocation = transform.position;
    }

    void IsMoving()
    {
        if (playerInSightRange || playerInAttackRange || isElite) return;

        if(Vector3.Distance(lastLocation, transform.position) < 0.5f) // 이동하지 않았음
        {
            walkPointSet = false;
            Debug.Log("이동 지점 재설정");
        }
    }

    void ChasePlayer()
    {
        if(isElite) // 문지기 및 보스 몬스터의 경우 캐릭터 컨트롤러로 이동
        {
            transform.LookAt(player);
            Vector3 direction = player.position - gameObject.transform.position;
            float distance = Vector3.Distance(player.position, gameObject.transform.position);
            direction.y = 0;

            if (distance > 2f)
            {
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                if (direction.magnitude > 1f)
                {
                    Vector3 movementVelocity = direction.normalized * chaseSpeed;
                    gameObject.GetComponent<CharacterController>().Move(movementVelocity * Time.deltaTime);
                }
            }
        }
        else //일반 몬스터는 단순하게 이동
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }

        if(isInCombat) isInCombat = false;
        
        targetFound = true;

        if (name.Contains("Slime") || name.Contains("Turtle") | name.Contains("Mushroom") || isElite)
        {
            animator.SetBool("See", true);
            if(name.Contains("Mushroom") || isElite) return;

            animator.SetBool("Walk", false);
            animator.SetBool("Stop", false);
            animator.SetBool("Battle", false);
        }

        animator.SetBool("Walk", true);
    }

    void AttackPlayer()
    {
        if(targetFound)
        {
            targetFound = false;
            animator.SetBool("See", false);
        }

        agent.SetDestination(transform.position); // 정지
        transform.LookAt(player);

        if (canAttack)
        {
            canAttack = false;

            if (isElite)
            {
                // 보스 전용 공격 로직
                int n = UnityEngine.Random.Range(1, 12);
                if (n < 6)
                {
                    animator.SetTrigger("Attack1");
                    // 보스 공격 타입 1
                }
                else if (n < 10)
                {
                    animator.SetTrigger("Attack2");
                    // 보스 공격 타입 2
                }
                else
                {
                    animator.SetTrigger("Attack3");
                    FireballAttack(); // 보스의 특별 공격
                }
            }
            else
            {
                // 일반 몬스터 공격 로직
                int n = UnityEngine.Random.Range(1, 10);
                if (n < 8)
                {
                    animator.SetTrigger("Attack1");
                    // 일반 공격 타입 1
                }
                else
                {
                    animator.SetTrigger("Attack2");
                    // 일반 공격 타입 2
                }
            }

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void FireballAttack()
    {
        fireball.SetActive(true);
        Invoke(nameof(FireballDeactivate), 1f);
        // 추가적인 보스 공격 로직
    }

    void FireballDeactivate()
    {
        fireball.SetActive(false);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("GetHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(isDead) return;

        isDead = true;
        animator.SetTrigger("Die");
        agent.enabled = false;
        // 추가적인 사망 처리 (아이템 드랍, 이벤트 트리거 등)
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // 공격
    }

}
