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
    protected bool isDead { get; set;}


    [Header("Attack Settings")]
    private bool canAttack = true;

    public virtual void Start()
    {
        playerTransform = GameObject.Find("Fox").transform;
        currentHealth = EnemyInfo.MaxHealth;
        animator = EnemyInfo.EnemyObject.GetComponent<Animator>();
    }

    void Update()
    {
        // 시야 및 공격 범위 체크
        PlayerInSightRange = Physics.CheckSphere(EnemyInfo.Transform.position, EnemyInfo.SightRange, LayerMask.NameToLayer("Player"));
        PlayerInAttackRange = Physics.CheckSphere(EnemyInfo.Transform.position, EnemyInfo.AttackRange, LayerMask.NameToLayer("Player"));

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
        EnemyInfo.EnemyObject.GetComponent<Animator>().SetBool("Walk", true);
    }

    public virtual void AttackPlayer()
    {
        if(targetFound)
        {
            targetFound = false;
            EnemyInfo.EnemyObject.GetComponent<Animator>().SetBool("See", false);
        }

        isInCombat = true;

        EnemyInfo.Transform.LookAt(playerTransform);

        if (canAttack)
        {
            canAttack = false;
        }

        if(PlayerInfoManager.instance.death)
        {
            isInCombat = false;
            animator.SetBool("Battle", false);
        }
    }

   
    void ResetAttack()
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
        if(isDead) return;

        isDead = true;
        animator.SetTrigger("Die");
        OwnerController.Invoke(nameof(OwnerController.Destroy), 2f);
    }
}
