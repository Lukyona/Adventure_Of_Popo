using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyController : MonoBehaviour, IEnemyController
{
    [SerializeField] private EnemyInfo enemyInfo;
    private EnemyCombatComponent combatComponent;

    public Animator Animator { get; private set; }
    private BoxCollider AttackCollider { get; set; }

    private NavMeshAgent agent;
    private Vector3 initialLocation;
    private Vector3 lastLocation;
    [SerializeField] private float walkPointRange;
    private Vector3 walkPoint;
    private bool walkPointSet = false;

    [SerializeField] private float patrolSpeed;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        AttackCollider = GetComponentInChildren<BoxCollider>();
        initialLocation = transform.position;

        combatComponent = new BasicEnemyCombatComponent()
        {
            Agent = agent,
            EnemyInfo = ScriptableObject.CreateInstance<EnemyInfo>()
        };

        //Json으로 바꾼 enemyInfo의 필드 값을 CombatComponent.EnemyInfo 필드 값으로 복사
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(enemyInfo), combatComponent.EnemyInfo);
        combatComponent.EnemyInfo.EnemyObject = gameObject; // 각 객체 오브젝트를 할당

        combatComponent.Start();
        InvokeRepeating(nameof(IsMoving), 3f, 3f);
    }

    private void Update()
    {
        if (IsDead()) return;

        if (combatComponent.EnemyInfo.EnemyObject == null)
        {
            combatComponent.EnemyInfo.EnemyObject = gameObject;
        }

        if (!combatComponent.PlayerInSightRange && !combatComponent.PlayerInAttackRange)
            Patrolling();

        combatComponent.Update();
    }

    private void Patrolling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        float distanceToWalkPoint = Vector3.Distance(transform.position, walkPoint);

        agent.speed = patrolSpeed;
        if (distanceToWalkPoint < 1f)
            walkPointSet = false;


        if (name.Contains("Slime") || name.Contains("Turtle") || name.Contains("Mushroom"))
        {
            if (combatComponent.targetFound)//발견했었지만 시야범위 벗어났을 때
            {
                combatComponent.targetFound = false;
                Animator.SetBool("See", false);
            }

            if (name.Contains("Mushroom")) return;

            Animator.SetBool("Walk", walkPointSet); // 슬라임/거북만 해당

            //walkPoint에 도달했으면, 슬라임/거북이만 실행
            if (!walkPointSet)
            {
                Animator.SetBool("Stop", true);
            }
            else
            {
                Animator.SetBool("Stop", false);
            }
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // 초기 위치 범위 내인지 체크
        if (Vector3.Distance(transform.position, initialLocation) > enemyInfo.MaxDistance)
        {
            walkPoint = initialLocation;
        }

        walkPointSet = true;
        lastLocation = transform.position;
    }

    private void IsMoving()
    {
        if (combatComponent.PlayerInSightRange || combatComponent.PlayerInAttackRange) return;

        if (Vector3.Distance(lastLocation, transform.position) < 1f) // 이동하지 않았음
        {
            walkPointSet = false;
        }
    }

    public int GetLevel()
    {
        return enemyInfo.Level;
    }

    public float GetMaxHealth()
    {
        return enemyInfo.MaxHealth;
    }

    public EnemyCombatComponent GetCombatComponent()
    {
        return combatComponent;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsDead()) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            PlayerCombatComponent pComp = Player.instance.CombatComponent;
            SoundManager.instance.PlayAttackSound(pComp.AttackNum);
            TakeDamage(pComp.SkillDamage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("NpcAttack"))
        {
            float damage = other.GetComponentInParent<FriendController>().SkillDamage;
            TakeDamage(damage);
        }
        other.GetComponent<BoxCollider>().enabled = false; // 중복 감지 방지
    }

    public void EnableAttackCollider()
    {
        AttackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        AttackCollider.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        combatComponent.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return combatComponent.IsDead;
    }

    public void Disable()
    {
        enabled = false;
    }

    public void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
