using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyCombatComponent
{
    public EnemyInfo EnemyInfo {get; set;}
    protected Animator animator;

    protected GameObject owner;

    protected Transform playerTransform;
    LayerMask whatIsPlayer;

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
        whatIsPlayer = LayerMask.GetMask("Player");
        owner = EnemyInfo.EnemyObject;
    }

    public virtual void Update()
    {
        // 시야 및 공격 범위 체크
        PlayerInSightRange = Physics.CheckSphere(owner.transform.position, EnemyInfo.SightRange, whatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(owner.transform.position, EnemyInfo.AttackRange, whatIsPlayer);
        if (currentHealth > 0 && !Player.instance.IsDead())
        {
            if (PlayerInSightRange && !PlayerInAttackRange)
                ChasePlayer();
            if (PlayerInSightRange && PlayerInAttackRange)
                AttackPlayer();
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

        owner.transform.LookAt(playerTransform);

        if(Player.instance.IsDead())
        {
            isInCombat = false;

            if(owner.name.Contains("Slime") || owner.name.Contains("Turtle"))
                animator.SetBool("Battle", false);
        }

        MyTaskManager.instance.ExecuteAfterDelay(ResetAttack, EnemyInfo.TimeBetweenAttacks);
    }

    public void ResetAttack()
    {
        canAttack = true;
        if(owner.name.Contains("Boss"))
        {
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
        }
        else
        {
            animator.ResetTrigger("Attack");
            if(!owner.name.Contains("Bat") && !owner.name.Contains("Mushroom"))
                animator.ResetTrigger("StrongAttack");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if(!owner.name.Contains("Bat") && !owner.name.Contains("Log") && !owner.name.Contains("Boss"))
            animator.SetTrigger("GetHit");

        UIManager.instance.ShowDamageText(owner, damage);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            EnemyHUD.instance.DecreaseHealthUI(owner.transform, damage);
        }
    }

    public virtual void Die()
    {
        IsDead = true;
        animator.SetTrigger("Die");

        MyTaskManager.instance.ExecuteAfterDelay(DestroyOwner, 2f);
    }

    void DestroyOwner()
    {
        owner.GetComponent<IEnemyController>().DestroyMyself();
    }
}
