using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EnemyCombatComponent
{
    protected Timer timer;

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
        
        targetFound = true;
    }

    public virtual void AttackPlayer()
    {
        canAttack = false;

        if(targetFound)
        {
            targetFound = false;
            EnemyInfo.EnemyObject.GetComponent<Animator>().SetBool("See", false);
        }

        isInCombat = true;

        EnemyInfo.EnemyObject.transform.LookAt(playerTransform);

        if(Player.instance.IsDead())
        {
            isInCombat = false;
            animator.SetBool("Battle", false);
        }

        timer = new Timer(_ => ResetAttack(), null, (int)EnemyInfo.TimeBetweenAttacks * 1000, Timeout.Infinite);
    }

    public void ResetAttack()
    {
        canAttack = true;
        timer.Dispose();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("GetHit");

        MonsterHPBar.instance.ShowDamage(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            MonsterHPBar.instance.Get_Damage(damage);
        }
    }

    public  virtual void Die()
    {
        IsDead = true;
        animator.SetTrigger("Die");

        timer = new Timer(_ => DestroyOwner(), null, 2000, Timeout.Infinite);
    }

    void DestroyOwner()
    {
        timer.Dispose();
        EnemyInfo.EnemyObject.GetComponent<IEnemyController>().DestroyMyself();
    }
}
