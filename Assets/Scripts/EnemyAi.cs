using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAi : MonoBehaviour
{
   NavMeshAgent agent;

   Transform player;

    public LayerMask whatIsPlayer;

    //몬스터 순찰
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;//시야범위, 공격범위
    public bool playerInSightRange, playerInAttackRange;

    Animator animator; //몬스터 애니메이터
    bool discover = false; //플레이어 발견했을 때 애니메이션이 한번만 실행되기 위함
    bool win = false; // 승리했을 때 true

    private void Awake()
    {
        player = GameObject.Find("Fox").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public float health_info;//초기 생명력 정보
    float firstPos_x;//초기 위치
    float firstPos_z;
    private void Start()
    {
        firstPos_x = gameObject.transform.position.x;//초기 위치 저장
        firstPos_z = gameObject.transform.position.z;
        InvokeRepeating(nameof(Check_Walk), 1f, 0.7f);
        health_info = health;//초기 생명력 저장
    }

    private void Update()
    {
        //몬스터 시야범위와 공격범위 체크
        playerInSightRange = Physics.CheckSphere(gameObject.transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(gameObject.transform.position, attackRange, whatIsPlayer);
        if(health > 0) //체력이 0보다 클 때
        {
            if (!playerInSightRange && !playerInAttackRange) //플레이어가 공격범위와 시야범위에 있지 않으면
            {
                Patroling(); //순찰하기
            }
            if (!PlayerInfoManager.instance.death)//플레이어가 살아있으면
            {
                win = false;
                if (playerInSightRange && !playerInAttackRange)//플레이어가 시야범위에 있지만 공격범위에는 없으면
                {
                    ChasePlayer();//플레이어 쫓기
                }
                if (playerInSightRange && playerInAttackRange)//시야범위, 공격범위에 플레이어가 있으면
                {
                    AttackPlayer();//플레이어 공격하기
                }
            }
            else
            {
                if (!win)
                {
                    win = true;
                    if (battle)
                    {
                        battle = false;
                        animator.SetBool("Battle", false);
                    }
                    if (gameObject.name.Contains("Slime") || gameObject.name.Contains("Turtle"))//거북이나 슬라임의 경우
                    {
                        animator.SetTrigger("Victory");
                    }

                }
            }
        }      
    }

    void Patroling()
    {
        
        if (!walkPointSet) //walkPoint가 지정되어 있지 않으면
        {
            SearchWalkPoint();//찾기
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = gameObject.transform.position - walkPoint;

        if (gameObject.name.Contains("Slime") || gameObject.name.Contains("Turtle"))//거북이나 슬라임의 경우
        {
            agent.speed = 0.8f;
            if (discover)//발견했었지만 시야범위 벗어났을 때
            {
                discover = false;
                animator.SetBool("See", false);
            }
            if (battle)//배틀 중이었다가 시야범위 벗어났을 때
            {
                battle = false;
                animator.SetBool("Battle", false);
            }

            //walkPoint에 도달했으면
            if (distanceToWalkPoint.magnitude < 1f)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Stop", true);
                walkPointSet = false;
            }
            else
            {
                animator.SetBool("Stop", false);
                animator.SetBool("Walk", true);
            }
        }
        else if(gameObject.name.Contains("Mushroom"))
        {
            agent.speed = 1.3f;
            if (distanceToWalkPoint.magnitude < 1f && walkPointSet)
            {
                walkPointSet = false;
            }
            if (discover)//발견했었지만 시야범위 벗어났을 때
            {
                discover = false;
                animator.SetBool("See", false);
            }
            if (battle)
            {
                battle = false;
            }
        }
        else
        {
            agent.speed = 2f;
            if (distanceToWalkPoint.magnitude < 1f && walkPointSet)
            {
                walkPointSet = false;
            }
            if (battle)
            {
                battle = false;
            }
        }
    }

     float now;//현재 위치
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        //임의의 walkPoint 지정
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        float dis_x = firstPos_x - gameObject.transform.position.x;//지정된 워크포인트와 초기 위치와의 거리 계산
        float dis_z = firstPos_z - gameObject.transform.position.z;
        if(dis_x < 10f && dis_x > -10f && dis_z < 10f && dis_z > -10f)//초기위치로부터 일정 범위 안이면 통과
        {
            walkPointSet = true;
            now = gameObject.transform.position.x;//현재 위치 저장
        }
        else
        {
            walkPoint = new Vector3(firstPos_x, transform.position.y, firstPos_z);
            walkPointSet = true;
        }
        now = gameObject.transform.position.x;//현재 위치 저장
    }

    void Check_Walk()//이동했는지 확인
    {
        if (!playerInSightRange && !playerInAttackRange && walkPointSet)
        {
            float distance = now - gameObject.transform.position.x;
            if (distance < 0.5f && distance > -0.5f)//계속 같은 위치이면 워크 포인트 재설정하기
            {
              //  Debug.Log("재설정");
                walkPointSet = false;
            }
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        if (gameObject.name.Contains("Slime") || gameObject.name.Contains("Turtle"))//거북이나 슬라임의 경우
        {
            agent.speed = 2f;
            animator.SetBool("Walk", false);
            animator.SetBool("Stop", false);
            if (!discover)//발견
            {
                animator.SetBool("See", true);
                discover = true; //발견한 상태
            }
            if (battle)
            {
                battle = false;
                animator.SetBool("Battle", false);
            }
        }
        else if (gameObject.name.Contains("Mushroom"))
        {
            agent.speed = 2f;
            if (!discover)//발견
            {
                animator.SetBool("See", true);
                discover = true; //발견한 상태
            }
            if (battle)
            {
                battle = false;
            }
        }
        else
        {
            agent.speed = 3f;
            if (battle)
            {
                battle = false;
            }
        }
    }

    public bool battle = false; //배틀 트리거도 한번씩만 실행되기 위함
    
    void AttackPlayer()
    {
        agent.SetDestination(gameObject.transform.position);//몬스터 안 움직이게
        gameObject.transform.LookAt(player);//플레이어를 바라보게

        if (gameObject.name.Contains("Slime") || gameObject.name.Contains("Turtle"))//거북이나 슬라임의 경우
        {
            discover = false;
            if (!battle)//배틀상태가 아닐 때
            {
                battle = true;//배틀 상태임
                animator.SetBool("See", false);
                animator.SetBool("Battle", true);
                Invoke(nameof(Attack), 1.3f);
            }
        } 
        else if(gameObject.name.Contains("Mushroom"))
        {
            discover = false;
            if (!battle)//배틀상태가 아닐 때
            {
                battle = true;//배틀 상태임
                animator.SetBool("See", false);
                Invoke(nameof(Attack), 1f);
            }
        }
        else
        {
            if (!battle)//배틀상태가 아닐 때
            {
                battle = true;//배틀 상태임
                Invoke(nameof(Attack), 1f);
            }
        }
    }

    void Attack()
    {
        if (!alreadyAttacked && playerInSightRange && playerInAttackRange && battle == true && health > 0)
        {
            int n = Random.Range(1, 10);

            if (n < 8) // 일반 공격, 확률 70%
            {
                 if (gameObject.name.Contains("Slime"))
                 {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.a = 1;//공격 종류 설정
                 }
                if (gameObject.name.Contains("Turtle"))
                {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.a = 3;//공격 종류 설정
                }
                if(gameObject.name.Contains("Log"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.a = 5;
                }
                if (gameObject.name.Contains("Bat"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.a = 7;
                }
                if (gameObject.name.Contains("Mushroom"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.a = 9;
                }
            }
            else if (n > 7) //센 공격, 확률 30%
            {
                if (gameObject.name.Contains("Slime"))
                {
                    animator.SetTrigger("Attack2");
                    PlayerInfoManager.instance.a = 2;
                }
                if (gameObject.name.Contains("Turtle"))
                {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.a = 4;//공격 종류 설정
                }
                if (gameObject.name.Contains("Log"))
                {
                    animator.SetTrigger("Attack2");
                    PlayerInfoManager.instance.a = 6;
                }
                if (gameObject.name.Contains("Bat"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.a = 8;
                }
                if (gameObject.name.Contains("Mushroom"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.a = 10;
                }
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            PlayerInfoManager.instance.Invoke(nameof(PlayerInfoManager.instance.PlayerDamage), 0.8f);//플레이어가 데미지 입음
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
        Attack();
    }

    public float health;//몬스터 생명력

    public void TakeDamage(int damage)//데미지 받는 함수
    {
        MonsterHPBar.instance.ShowDamage(damage);
        if (gameObject.name.Contains("Slime") || gameObject.name.Contains("Turtle") || gameObject.name.Contains("Mushroom"))
        {
            animator.SetTrigger("GetHit");
        }
        health -= damage;

        MonsterHPBar.instance.Get_Damage(damage);
        if (health <= 0)
        {
            if(gameObject.name.Contains("Dragon"))//보스 죽음
            {
                SoundManager.instance.PlayDragonDieSound();//효과음 재생
                GameDirector.instance.Invoke(nameof(GameDirector.instance.AfterDragonDead),0.5f);//대화 준비
            }
            else
            {
                PlayerInfoManager.instance.Invoke(nameof(PlayerInfoManager.instance.GetEXP), 1.5f);
            }
            animator.SetTrigger("Die");
            Invoke(nameof(DestroyEnemy), 2f);
            MonsterHPBar.instance.DisappearMonsterInfo();
        }
    }


    void DestroyEnemy()//몬스터 죽음, 오브젝트 파괴, 몬스터 수 감소
    {        
        if(gameObject.name.Contains("Slime"))
        {
            if (gameObject.name.Contains("3")) 
            {
                PlayerInfoManager.instance.slime2 -= 1;//슬라임 수 1 감소
            }
            else
            {
                PlayerInfoManager.instance.slime1 -= 1;//슬라임 수 1 감소
            }
        }
        if (gameObject.name.Contains("Turtle"))
        {
            PlayerInfoManager.instance.turtle -= 1;//거북이 수 1 감소
        }
        if (gameObject.name.Contains("Log"))
        {
            PlayerInfoManager.instance.tree -= 1;//나무 수 1 감소
        }
        if (gameObject.name.Contains("Bat"))
        {
            PlayerInfoManager.instance.bat -= 1;//박쥐 수 1 감소
        }
        if (gameObject.name.Contains("Mushroom"))
        {
            PlayerInfoManager.instance.mushroom -= 1;//버섯 수 1 감소
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; //공격범위 레드로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);

        Gizmos.color = Color.green; //시야범위 그린으로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, sightRange);
    }
}
