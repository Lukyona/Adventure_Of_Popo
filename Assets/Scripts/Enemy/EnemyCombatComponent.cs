using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatComponent
{
    public EnemyInfo EnemyInfo {get; set;}
    protected Animator animator;
    protected Transform playerTransform;

    public bool targetFound {get; set;}
    protected bool isInCombat = false;
    public bool PlayerInSightRange { get; private set;}
    public bool PlayerInAttackRange { get; private set;}

    [Header("Health Settings")]
    protected float currentHealth;
    public bool IsDead { get; protected set;}


    [Header("Attack Settings")]
    protected bool canAttack = true;
    public float SkillDamage { get; protected set;}


    public virtual void Start()
    {
        playerTransform = GameObject.Find("Fox").transform;
        currentHealth = EnemyInfo.MaxHealth;
        animator = EnemyInfo.EnemyObject.GetComponent<Animator>();
    }

    public virtual void Update()
    {
        // 시야 및 공격 범위 체크
        PlayerInSightRange = Physics.CheckSphere(EnemyInfo.EnemyObject.transform.position, EnemyInfo.SightRange, LayerMask.GetMask("Player"));
        PlayerInAttackRange = Physics.CheckSphere(EnemyInfo.EnemyObject.transform.position, EnemyInfo.AttackRange, LayerMask.GetMask("Player"));
        if (currentHealth > 0)
        {
            if (PlayerInSightRange && !PlayerInAttackRange)
                ChasePlayer();
            if (PlayerInSightRange && PlayerInAttackRange)
                AttackPlayer();
        }
        else
        {
            Die();
        }
    }

    public virtual void ChasePlayer()
    {
        if(isInCombat) isInCombat = false;
       // Debug.Log("ChasePlayer")   ;
        targetFound = true;
    }

    public virtual void AttackPlayer()
    {
        canAttack = false;
       // Debug.Log("AttackPlayer")   ;

        if(targetFound)
        {
            targetFound = false;
        }
        
        
        isInCombat = true;

        EnemyInfo.EnemyObject.transform.LookAt(playerTransform);

        if(Player.instance.IsDead())
        {
            isInCombat = false;

            if(EnemyInfo.EnemyObject.name.Contains("Slime") || EnemyInfo.EnemyObject.name.Contains("Turtle"))
                animator.SetBool("Battle", false);
        }

        MyTaskManager.instance.ExecuteAfterDelay(ResetAttack, EnemyInfo.TimeBetweenAttacks);
    }

    public void ResetAttack()
    {
        canAttack = true;
        animator.ResetTrigger("Attack");

        if(!EnemyInfo.EnemyObject.name.Contains("Bat") && !EnemyInfo.EnemyObject.name.Contains("Mushroom"))
            animator.ResetTrigger("StrongAttack");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if(!EnemyInfo.EnemyObject.name.Contains("Bat") && !EnemyInfo.EnemyObject.name.Contains("Log"))
            animator.SetTrigger("GetHit");

        animator.ResetTrigger("Attack");

        if(!EnemyInfo.EnemyObject.name.Contains("Bat") && !EnemyInfo.EnemyObject.name.Contains("Mushroom"))
            animator.ResetTrigger("StrongAttack");

        MonsterHPBar.instance.ShowDamage(EnemyInfo.EnemyObject.transform, damage);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            MonsterHPBar.instance.Get_Damage(EnemyInfo.EnemyObject.transform, damage);
        }
    }

    public  virtual void Die()
    {
        IsDead = true;
        animator.SetTrigger("Die");

        MyTaskManager.instance.ExecuteAfterDelay(DestroyOwner, 2f);
    }

    void DestroyOwner()
    {
        EnemyInfo.EnemyObject.GetComponent<IEnemyController>().DestroyMyself();
    }
}
