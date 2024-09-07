using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatComponent
{
    public EnemyInfo EnemyInfo {get; set;}
    protected Animator animator;
    public MonoBehaviour OwnerController {get; set;}
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

        if(PlayerInfoManager.instance.death)
        {
            isInCombat = false;
            animator.SetBool("Battle", false);
        }
    }

    public void ResetAttack()
    {
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("GetHit");

        MonsterHPBar.instance.ShowDamage(damage);
        MonsterHPBar.instance.Get_Damage(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if(IsDead) return;

        IsDead = true;
        animator.SetTrigger("Die");
        OwnerController.Invoke(nameof(OwnerController.Destroy), 2f);
    }
}
