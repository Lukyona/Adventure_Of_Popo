using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendController : MonoBehaviour
{
    Animator animator;
    bool isSlime;

    Transform player;
    bool playerInRange;

    bool enemyInRange;


    Vector3 movementVelocity;

    public bool IsInCombat {get; set;} //플레이어가 전투 중이면 true
    public Transform CombatTarget {get; set;}

    BoxCollider attackCollider;

    public float SkillDamage {get; private set;}

    void Start()
    {
        player = Player.instance.transform;
        animator = gameObject.GetComponent<Animator>();
        isSlime = gameObject.name.Contains("Slime");
        attackCollider = GetComponentInChildren<BoxCollider>();
    }

    void Update()
    {
        playerInRange = Physics.CheckSphere(gameObject.transform.position, 6f, LayerMask.GetMask("Player"));

        if(GameDirector.instance.mainCount >= 9)
        {
            enemyInRange = Physics.CheckSphere(gameObject.transform.position, 5f, LayerMask.GetMask("Enemy"));
            if(enemyInRange)
            {
                IsInCombat = true;
            }
        }

        if (!IsInCombat || (IsInCombat && !playerInRange && GameDirector.instance.mainCount < 9))//전투 중이 아니거나 전투 중이어도 플레이어가 일정 거리 멀어지면 플레이어에게로 이동, 보스전은 해당X
        {
            if(IsInCombat) IsInCombat = false;
            if(CombatTarget) CombatTarget = null;

            MoveToPlayer();
        }
        else//전투 상태, 플레이어와 가까움
        {
            MoveToTarget();
        }  
    }

    void UpdateSlimeAnimationState(bool isWalking = false, bool isStopped = false, bool isInBattle = false)
    {
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Stop", isStopped);
        animator.SetBool("Battle", isInBattle);
    }
    
    void MoveToPlayer()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 direction = player.position - transform.position;
       
        direction.y = 0;
        if (isSlime)
        {
            UpdateSlimeAnimationState(isWalking : true);
        }

        if (direction.magnitude > 1)
        {
            if (distance > 10)
            {
                MoveTo(player, 5.5f);//플레이어와 멀리 떨어져있을 때
            }
            else
            {
                MoveTo(player, 3.7f);//비교적 가까울 때
            }

            if(distance > 40) //너무 멀면 근처로 순간이동 
            {
                float random = Random.Range(-5.0f, 5.0f);
                transform.position = new Vector3(player.position.x + random, player.position.y, player.position.z + random);
            }
        }
    }

    void MoveToTarget()
    {
        if(CombatTarget == null) CombatTarget = Player.instance.GetTarget()?.transform;
        if (GameDirector.instance.mainCount == 9)
        {
            CombatTarget = GameObject.Find("Monster_DogKnight")?.transform; 
        }
        if (GameDirector.instance.mainCount == 10)
        {
            CombatTarget = GameObject.Find("Monster_Dragon_Boss")?.transform; // 보스전일 때
        }
        
        if(CombatTarget == null) return;

        if(CombatTarget.GetComponentInParent<IEnemyController>().IsDead()) return;

        MoveTo(CombatTarget, 5f);
    }

    void MoveTo(Transform target, float speed)
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;

        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget > 2)
        {
            Vector3 targetPos = target.transform.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);

            if (directionToTarget.magnitude > 1)
            {
                movementVelocity = directionToTarget.normalized * speed;
                movementVelocity.y = Mathf.Clamp(GetComponent<CharacterController>().velocity.y, -30, -2);
                GetComponent<CharacterController>().Move(movementVelocity * Time.deltaTime);
            }
        }
        else
        {
            if (isSlime)
            {
                UpdateSlimeAnimationState(isStopped : true);
            }

            if(CombatTarget)
            {
                IsInCombat = true;
                if (isSlime)
                {
                    UpdateSlimeAnimationState(isInBattle : true);
                }
                
                Invoke(nameof(Attack),0.5f);
            }
        }
    }

    void Attack()
    {
        if (IsInCombat && CombatTarget)//타겟이 있거나 없어도 보스전일 때
        {
            if(Player.instance.IsDead() || CombatTarget.GetComponent<IEnemyController>().IsDead()) 
            {
                IsInCombat = false;//동료 전투 상태 해제
                CombatTarget = null;
            }
            else
            {
                int n = Random.Range(1, 10);

                if (isSlime)
                {
                    animator.SetTrigger(n < 8 ? "Attack" : "StrongAttack");
                    SkillDamage = n < 8 ? 5f : 10f;
                }
                else
                {
                    animator.SetTrigger("Attack");
                    SkillDamage = n < 8 ? 7f : 13f;
                }
                Invoke(nameof(Attack), 2f);//2초 후 재공격
            }            
        }        
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

}
