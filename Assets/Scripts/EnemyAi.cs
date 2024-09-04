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



    private void Update()
    {
        //몬스터 시야범위와 공격범위 체크

        if(health > 0) //체력이 0보다 클 때
        {
          
            if (!PlayerInfoManager.instance.death)//플레이어가 살아있으면
            {
                win = false;

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
                    PlayerInfoManager.instance.PlayerDamage(7);
                 }
                if (gameObject.name.Contains("Turtle"))
                {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.PlayerDamage(10);
                }
                if(gameObject.name.Contains("Log"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.PlayerDamage(15);
                }
                if (gameObject.name.Contains("Bat"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.PlayerDamage(20);
                }
                if (gameObject.name.Contains("Mushroom"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.PlayerDamage(23);
                }
            }
            else if (n > 7) //센 공격, 확률 30%
            {
                if (gameObject.name.Contains("Slime"))
                {
                    animator.SetTrigger("Attack2");
                    PlayerInfoManager.instance.PlayerDamage(15);
                }
                if (gameObject.name.Contains("Turtle"))
                {
                    animator.SetTrigger("Attack1");
                    PlayerInfoManager.instance.PlayerDamage(20);//공격 종류 설정
                }
                if (gameObject.name.Contains("Log"))
                {
                    animator.SetTrigger("Attack2");
                    PlayerInfoManager.instance.PlayerDamage(25);
                }
                if (gameObject.name.Contains("Bat"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.PlayerDamage(27);
                }
                if (gameObject.name.Contains("Mushroom"))
                {
                    animator.SetTrigger("Attack");
                    PlayerInfoManager.instance.PlayerDamage(33);
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
