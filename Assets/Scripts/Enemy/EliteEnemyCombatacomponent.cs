using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EliteEnemyCombatacomponent : EnemyCombatComponent
{
    Vector3 initialLocation;
    Quaternion initialRotation;
    bool isReturning = false;

    public GameObject Fireball {private get; set;} // 보스의 특별 공격용 오브젝트


    public override void Start()
    {
        base.Start();
        initialLocation = EnemyInfo.EnemyObject.transform.position;
        initialRotation = EnemyInfo.EnemyObject.transform.rotation;
    }

    public override void Update()
    {
        base.Update();

        if(isReturning) RetrunToInitialLocation();
    }

    public override void ChasePlayer()
    {   
        if(isReturning) return;

        animator.SetBool("See", true);

        Vector3 target = playerTransform.position;

        Vector3 direction = target - owner.transform.position;
        float distance = Vector3.Distance(target, owner.transform.position);
        direction.y = 0;

        target.y =owner.transform.position.y;
        owner.transform.LookAt(target);
        
        if (distance > 1f)
        {
            if (direction.magnitude > 1f)
            {
                Vector3 movementVelocity = direction.normalized * EnemyInfo.ChaseSpeed;
                movementVelocity.y = Mathf.Clamp(owner.GetComponent<CharacterController>().velocity.y, -30, -2);
                owner.GetComponent<CharacterController>().Move(movementVelocity * Time.deltaTime);
            }
        }

        base.ChasePlayer(); // 부모 메서드 호출

        if (Vector3.Distance(owner.transform.position, initialLocation) > EnemyInfo.MaxDistance) 
            RetrunToInitialLocation();
    }

    void RetrunToInitialLocation()
    {
        isReturning = true;

        targetFound = false;
        float distance = Vector3.Distance(initialLocation, owner.transform.position);
        Vector3 direction = initialLocation - owner.transform.position;
        direction.y = 0;
        owner.transform.LookAt(initialLocation);

        if (distance > 2f)
        {
            if (direction.magnitude > 1f)
            {
                Vector3 movementVelocity = direction.normalized * EnemyInfo.ChaseSpeed;
                movementVelocity.y = Mathf.Clamp(owner.GetComponent<CharacterController>().velocity.y, -30, -2);
                owner.GetComponent<CharacterController>().Move(movementVelocity * Time.deltaTime);
            }
        }
        else // 초기 위치에 거의 다다랐다면
        {
            isReturning = false;
            animator.SetBool("See", false);
            currentHealth = EnemyInfo.MaxHealth;
            EnemyHUD.instance.ResetHP();
            
            owner.transform.rotation = initialRotation;
        }
    }

    public override void AttackPlayer()
    {
        if(!canAttack) return;

        base.AttackPlayer();
        animator.SetBool("See", false);

        if (owner.name.Contains("Boss"))
        {
            // 보스 전용 공격 로직
            int n = Random.Range(1, 12);
            if (n < 6)
            {
                animator.SetTrigger("Attack1");
                SkillDamage = EnemyInfo.NormalAttackDamage;
                // 보스 공격 타입 1
            }
            else if (n < 10)
            {
                animator.SetTrigger("Attack2");
                SkillDamage = (EnemyInfo.NormalAttackDamage + EnemyInfo.StrongAttackDamage) / 2;
                // 보스 공격 타입 2
            }
            else
            {
                animator.SetTrigger("Attack3");
                SkillDamage = EnemyInfo.StrongAttackDamage;
                ActivateFireball(); // 보스의 특별 공격
            }
        }
        else
        {
            int n = Random.Range(1, 10);
            if (n < 8)
            {
                animator.SetTrigger("Attack"); // 일반 공격
                SkillDamage = EnemyInfo.NormalAttackDamage;
            }
            else
            {
                animator.SetTrigger("StrongAttack");
                SkillDamage = EnemyInfo.StrongAttackDamage;
            }
        }
    }

    public  void ActivateFireball()
    {
        Fireball.SetActive(true);
        MyTaskManager.instance.ExecuteAfterDelay(DeactivateFireball, 1f);
    }

    void DeactivateFireball()
    {
        Fireball.SetActive(false);
    }

    public override void Die()
    {
        if(IsDead) return;

        if(owner.name.Contains("Boss"))
        {
            SoundManager.instance.PlayDragonDieSound();
            GameDirector.instance.Invoke(nameof(GameDirector.instance.AfterDragonDead),0.5f); //대화 준비
        }
        else
        {
            Player.instance.StatusComponent.GetEXP(GameDirector.instance.GetObjectName(owner.name));
        }

        base.Die();
    }
}
