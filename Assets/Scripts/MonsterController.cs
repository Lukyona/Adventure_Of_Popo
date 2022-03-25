using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterController : MonoBehaviour//보스와 문지기 컨트롤러
{
    Transform player;
    public Animator animator;
    public LayerMask whatIsPlayer;

    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded;
    Vector3 velocity;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;//시야범위, 공격범위
    public bool playerInSightRange, playerInAttackRange;

    bool discover = false; //플레이어 발견했을 때 애니메이션이 한번만 실행되기 위함
   
    public float health_info;//초기 생명력 정보
    public float health;//현재 생명력
    public Transform firstPos;//초기 위치
    bool goBack = false;

    private void Awake()
    {
        player = GameObject.Find("Fox").transform;
        animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        health_info = health;//초기 생명력 저장
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, ThirdPlayerMovement.instance.groundDistance, groundMask);
        if (isGrounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += ThirdPlayerMovement.instance.gravity * Time.deltaTime; //중력 적용
        
        //시야범위와 공격범위 체크
        playerInSightRange = Physics.CheckSphere(gameObject.transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(gameObject.transform.position, attackRange, whatIsPlayer);

        if (health > 0) //체력이 0보다 클 때
        {
            if (!PlayerInfoManager.instance.death)//플레이어가 살아있으면
            {
                if (!goBack && (firstPos.position.x - gameObject.transform.position.x) <= 35 && (firstPos.position.z - gameObject.transform.position.z) <= 40)//초기 위치에서 일정 범위 이내면
                {
                    if (!playerInSightRange && !playerInAttackRange) //플레이어가 공격범위와 시야범위에 있지 않으면
                    {
                        if (firstPos.position != gameObject.transform.position)//초기위치가 아니면
                        {
                            GoBack();//초기 위치로 돌아감
                        }
                    }
                    if (playerInSightRange && !playerInAttackRange)//플레이어가 시야범위에 있지만 공격범위에는 없으면
                    {
                        ChasePlayer();//플레이어 쫓기                      
                    }
                    if (playerInSightRange && playerInAttackRange)//시야범위, 공격범위에 플레이어가 있으면
                    {
                        AttackPlayer();//플레이어 공격하기
                    }
                }
                else//초기 위치에서 일정 범위 밖이면 다시 초기 위치로 복귀
                {
                    goBack = true;
                    GoBack();
                }
            }
            else//플레이어가 죽으면 초기 위치로 복귀
            {
                GoBack();
            }
        }
    }

    float distance;//플레이어와의 거리
    public Vector3 ctrlVelocity;
    private void ChasePlayer()
    {
        if(!discover)
        {
            discover = true;
            animator.SetBool("See", true);
        }
        gameObject.transform.LookAt(player);
        Vector3 direction = player.position - gameObject.transform.position;
        distance = Vector3.Distance(player.position, gameObject.transform.position);
        direction.y = 0;

        if (distance > 2)
        {
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            if (direction.magnitude > 1)
            {
                ctrlVelocity = direction.normalized * 3.5f;//3.5f 스피드
                ctrlVelocity.y = Mathf.Clamp(gameObject.GetComponent<CharacterController>().velocity.y, -30, 2);
                ctrlVelocity.y -= 30.5f * Time.deltaTime; //중력 적용
                gameObject.GetComponent<CharacterController>().Move(ctrlVelocity * Time.deltaTime);//이동
            }
        }
    }

    private void AttackPlayer()
    {
        if(discover)
        {
            discover = false;
            animator.SetBool("See", false);
        }
        gameObject.transform.LookAt(player);//플레이어를 바라보게
        Invoke(nameof(Attack), 1f);
    }

    void Attack()
    {
        if (!alreadyAttacked && playerInSightRange && playerInAttackRange && health > 0)
        {
            if (gameObject.name.Contains("Dog"))
            {
                int n = Random.Range(1, 10);

                if (n < 8) // 일반 공격, 확률 70% 
                {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.a = 11;//공격 종류 설정
                }
                else if (n > 7) //센 공격, 확률 30%
                {
                    animator.SetTrigger("Attack2");
                    PlayerInfoManager.instance.a = 12;
                }
            } 
            if(gameObject.name.Contains("Dragon"))
            {
                int n = Random.Range(1, 12);

                if (n < 6) //1~5
                {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.a = 13;//공격 종류 설정
                }
                else if (n < 10) //6~9
                {
                    animator.SetTrigger("Attack2");
                    PlayerInfoManager.instance.a = 14;
                }
                else if (n <= 12) //10~12
                {
                    animator.SetTrigger("Attack3");
                    Invoke(nameof(FireballOn), 0.15f);
                    PlayerInfoManager.instance.a = 15;
                    Invoke(nameof(FireballOff), 1f);
                }
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            PlayerInfoManager.instance.Invoke(nameof(PlayerInfoManager.instance.PlayerDamage), 0.8f);//플레이어가 데미지 입음
        }
    }

    public GameObject fireball;//보스의 3번째 공격 시 나타날 파티클
    void FireballOn()//활성화를 할 때 효과가 나타나므로
    {
        fireball.SetActive(true);
    }

    void FireballOff()//다음에 또 효과가 나타나기 위해 비활성화
    {
        fireball.SetActive(false);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
        Attack();
    }

    public void TakeDamage(int damage)//데미지 받는 함수
    {
        MonsterHPBar.instance.ShowDamage(damage);
        if (gameObject.name.Contains("Dog"))
        {
            animator.SetTrigger("GetHit");
        }
        health -= damage;

        MonsterHPBar.instance.Get_Damage(damage);
        if (health <= 0)
        {
            if(gameObject.name.Contains("Dragon"))//보스 죽음
            {
                GameDirector.instance.DragonSound2();//효과음 재생
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
        Destroy(gameObject);
    }

    void GoBack()
    {
        gameObject.transform.LookAt(firstPos);
        Vector3 direction = firstPos.position - gameObject.transform.position;
        distance = Vector3.Distance(firstPos.position, gameObject.transform.position);
        direction.y = 0;
        if (distance > 1)//거리가 1보다 크면
        {
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(direction), 0.1f);

            ctrlVelocity = direction.normalized * 3.5f;//3.5f 스피드
            ctrlVelocity.y = Mathf.Clamp(gameObject.GetComponent<CharacterController>().velocity.y, -30, 2);
            ctrlVelocity.y -= 30.5f * Time.deltaTime; //중력 적용
            gameObject.GetComponent<CharacterController>().Move(ctrlVelocity * Time.deltaTime);//이동    
        }
        else
        {
            if (discover)
            {
                discover = false;
                animator.SetBool("See", false);
                health += 100;//체력 회복
                if (health >= health_info)//원래 체력보다 높아지면
                {
                    health = health_info;//초기화
                }
                MonsterHPBar.instance.Recover_HP(100);
            }
            gameObject.transform.rotation = firstPos.rotation;
            goBack = false;
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; //공격범위 레드로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);

        Gizmos.color = Color.green; //시야범위 그린으로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, sightRange);
    }
}
