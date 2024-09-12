using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendController : MonoBehaviour
{
    public static FriendController instance;

    Transform player;
    Animator animator; //슬라임/버섯 애니메이터
    public LayerMask whatIsPlayer;
    public bool playerInRange;

    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded;
    Vector3 velocity;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        player = GameObject.Find("Fox").transform;
        animator = gameObject.GetComponent<Animator>();
    }

    public bool battle = false;//플레이어가 전투 중이면 true
    bool attack_start = false;//공격 시작하면 true
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, ThirdPlayerMovement.instance.groundDistance, groundMask);
        if (isGrounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += ThirdPlayerMovement.instance.gravity * Time.deltaTime; //중력 적용

        playerInRange = Physics.CheckSphere(gameObject.transform.position, 6f, whatIsPlayer);

        if (!battle || (battle && !playerInRange && GameDirector.instance.mainCount != 10))//전투 중이 아니거나 전투 중이어도 플레이어가 일정 거리 멀어지면 플레이어에게로 이동, 보스전은 해당X
        {
            if(battle)
            {
                battle = false;
                attack_start = false;
            }
            npcMoving();
        }
        else//전투 상태, 플레이어와 가까움
        {
            if(Player.instance.GetTarget() != null)//타겟에게 이동
            {
                gameObject.transform.LookAt(Player.instance.GetTarget().transform);
                Vector3 direction = Player.instance.GetTarget().transform.position - gameObject.transform.position;
                distance = Vector3.Distance(Player.instance.GetTarget().transform.position, gameObject.transform.position);
                direction.y = 0;

                if (distance > 2)
                {
                    gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                    if (direction.magnitude > 1)
                    {
                        ctrlVelocity = direction.normalized * 5f;
                        ctrlVelocity.y = Mathf.Clamp(gameObject.GetComponent<CharacterController>().velocity.y, -30, 2);
                        ctrlVelocity.y -= 30.5f * Time.deltaTime; //중력 적용
                        gameObject.GetComponent<CharacterController>().Move(ctrlVelocity * Time.deltaTime);//이동
                    }
                }
            }
            else if(GameDirector.instance.mainCount == 10)//타겟 설정은 되지 않았지만 보스전일 때
            {
                gameObject.transform.LookAt(MonsterHPBar.instance.boss.transform);
                Vector3 direction = MonsterHPBar.instance.boss.transform.position - gameObject.transform.position;
                distance = Vector3.Distance(MonsterHPBar.instance.boss.transform.position, gameObject.transform.position);
                direction.y = 0;

                if (distance > 2)
                {
                    gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                    if (direction.magnitude > 1)
                    {
                        ctrlVelocity = direction.normalized * 5f;
                        ctrlVelocity.y = Mathf.Clamp(gameObject.GetComponent<CharacterController>().velocity.y, -30, 2);
                        ctrlVelocity.y -= 30.5f * Time.deltaTime; //중력 적용
                        gameObject.GetComponent<CharacterController>().Move(ctrlVelocity * Time.deltaTime);//이동
                    }
                }
            }
           
            if (!attack_start)
            {
                attack_start = true;
                if(gameObject.name.Contains("Slime"))
                {
                    animator.SetBool("Stop", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Battle", true);
                }
                Invoke(nameof(Attack),1f);
            }
        }    
    }

    float distance; //플레이어와 동료몬스터 사이 거리
    public void npcMoving()//이동
    {
        Vector3 direction = player.position - gameObject.transform.position;
        distance = Vector3.Distance(player.position, gameObject.transform.position);

        if ((ThirdPlayerMovement.instance.isPlayerAction == true && distance > 10) || (ThirdPlayerMovement.instance.isPlayerAction == false && distance > 3))
        {
            direction.y = 0;
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            if (direction.magnitude > 1)
            {

                if ((ThirdPlayerMovement.instance.running || ThirdPlayerMovement.instance.isPlayerIdle || !ThirdPlayerMovement.instance.running) && distance > 10)
                {
                    npcMove(direction, 5.5f);//플레이어와 멀리 떨어져있을 때
                }
                else
                {
                    npcMove(direction, 3.7f);//비교적 가까울 때
 
                }

                if(distance > 40) //너무 멀면 근처로 순간이동 
                {
                    float random = Random.Range(-5.0f, 5.0f);
                    transform.position = new Vector3(player.position.x + random, player.position.y, player.position.z + random);
                }
            }
        }
        else
        {
            if (gameObject.name.Contains("Slime"))
            {
                animator.SetBool("Battle", false);
                animator.SetBool("Walk", false);
                animator.SetBool("Stop", true);
            }
        }
    }

    public Vector3 ctrlVelocity;
    void npcMove(Vector3 dir, float spd)//이동 함수
    {
        if (gameObject.name.Contains("Slime"))
        {
            animator.SetBool("Battle", false);
            animator.SetBool("Stop", false);
            animator.SetBool("Walk", true);
        }

        ctrlVelocity = dir.normalized * spd;
        ctrlVelocity.y = Mathf.Clamp(gameObject.GetComponent<CharacterController>().velocity.y, -30, 2);
        ctrlVelocity.y -= 30.5f * Time.deltaTime; //중력 적용
        gameObject.GetComponent<CharacterController>().Move(ctrlVelocity * Time.deltaTime);//이동
    }

    void Attack()
    {
        if (battle && attack_start && ((Player.instance.GetTarget() != null) || (GameDirector.instance.mainCount == 10)))//타겟이 있거나 없어도 보스전일 때
        {
            if (GameDirector.instance.mainCount < 9 && Player.instance.GetTarget().GetComponent<IEnemyController>().IsDead())//타겟 죽었을 때
            {
                battle = false;//동료 전투 상태 해제
                Player.instance.SetTarget(null);
            }
            else if((GameDirector.instance.mainCount == 9 && Player.instance.GetTarget().GetComponent<IEnemyController>().IsDead()) 
                || (GameDirector.instance.mainCount == 10 && MonsterHPBar.instance.boss.GetComponent<IEnemyController>().IsDead()) || Player.instance.IsDead())
            {//보스전이어도 플레이어 죽으면 전투 해제
                battle = false;//동료 전투 상태 해제
                Player.instance.SetTarget(null);
            }
            else
            {
                if (gameObject.name.Contains("Slime"))//슬라임 동료면
                {
                    int n = Random.Range(1, 10);

                    if (n < 8) // 일반 공격, 확률 70%, 5데미지
                    {
                        animator.SetTrigger("Attack1");
                        Invoke(nameof(Slime_GiveDamage1), 0.4f);
                    }
                    else if (n > 7) //센 공격, 확률 30%, 10데미지
                    {
                        animator.SetTrigger("Attack2");
                        Invoke(nameof(Slime_GiveDamage2), 0.4f);
                    }
                }
                else if(gameObject.name.Contains("Mushroom"))
                {
                    animator.SetTrigger("Attack");
                    Invoke(nameof(Mush_GiveDamage), 0.5f);
                }
                Invoke(nameof(Attack), 2f);//2초 후 재공격
            }            
        }        
        else
        {
            battle = false;
            attack_start = false;
        }
    }

    void Slime_GiveDamage1()
    {
        if (Player.instance.GetTarget() != null)
        {
            if(GameDirector.instance.mainCount < 9)
            {
                Player.instance.GetTarget().GetComponent<IEnemyController>().TakeDamage(5);
            }
            else
            {
                Player.instance.GetTarget().GetComponent<IEnemyController>().TakeDamage(5);
            }
        }
        else if (GameDirector.instance.mainCount == 10)//타겟 설정은 되지 않았지만 보스전일 때
        {
            if (!MonsterHPBar.instance.boss.GetComponent<IEnemyController>().IsDead())//보스 살아있을 때
            {
                MonsterHPBar.instance.boss.GetComponent<IEnemyController>().TakeDamage(5);
            }
        }
    }

    void Slime_GiveDamage2()
    {
        if (Player.instance.GetTarget() != null)
        {
            if (GameDirector.instance.mainCount < 9)
            {
                Player.instance.GetTarget().GetComponent<IEnemyController>().TakeDamage(10);
            }
            else
            {
                Player.instance.GetTarget().GetComponent<IEnemyController>().TakeDamage(10);
            }
        }
        else if (GameDirector.instance.mainCount == 10)//타겟 설정은 되지 않았지만 보스전일 때
        {
            if (!MonsterHPBar.instance.boss.GetComponent<IEnemyController>().IsDead())
            {
                MonsterHPBar.instance.boss.GetComponent<IEnemyController>().TakeDamage(10);
            }
        }
    }
    

    void Mush_GiveDamage()
    {
        if (Player.instance.GetTarget() != null)
        {
            if (GameDirector.instance.mainCount < 9)
            {
                Player.instance.GetTarget().GetComponent<IEnemyController>().TakeDamage(12);
            }
            else
            {
                Player.instance.GetTarget().GetComponent<IEnemyController>().TakeDamage(12);
            }
        }
        else if (GameDirector.instance.mainCount == 10)//타겟 설정은 되지 않았지만 보스전일 때
        {
            if(!MonsterHPBar.instance.boss.GetComponent<IEnemyController>().IsDead())
            {
                MonsterHPBar.instance.boss.GetComponent<IEnemyController>().TakeDamage(12);
            }
        }
    }
}
