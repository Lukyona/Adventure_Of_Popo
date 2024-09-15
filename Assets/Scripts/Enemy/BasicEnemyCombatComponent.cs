using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Build.Content;


public class BasicEnemyCombatComponent : EnemyCombatComponent
{
    public NavMeshAgent Agent { get; set;}

    public override void ChasePlayer()
    {   
        base.ChasePlayer(); // 부모 메서드 호출

        Agent.speed = EnemyInfo.ChaseSpeed;
        Agent.SetDestination(playerTransform.position);

        if (EnemyInfo.EnemyObject.name.Contains("Slime") || EnemyInfo.EnemyObject.name.Contains("Turtle") | EnemyInfo.EnemyObject.name.Contains("Mushroom"))
        {
            animator.SetBool("See", true);
            if(EnemyInfo.EnemyObject.name.Contains("Mushroom")) return;

            animator.SetBool("Walk", false);
            animator.SetBool("Stop", false);
            animator.SetBool("Battle", false);
        }
    }

    public override void AttackPlayer()
    {
        if(!canAttack) return;

        if(EnemyInfo.EnemyObject.name.Contains("Slime") || EnemyInfo.EnemyObject.name.Contains("Turtle"))
        {
            animator.SetBool("Battle", true);

            if(Player.instance.IsDead())
            {
                animator.SetTrigger("Victory");
            }
        }

        base.AttackPlayer();

        Agent.SetDestination(EnemyInfo.EnemyObject.transform.position); // 정지
        
        int n = Random.Range(1, 10);
        if (n < 8)
        {
            animator.SetTrigger("Attack"); // 일반 공격
            SkillDamage = EnemyInfo.NormalAttackDamage;
        }
        else
        {
            if(EnemyInfo.EnemyObject.name.Contains("Bat") || EnemyInfo.EnemyObject.name.Contains("Mushroom"))
                animator.SetTrigger("Attack");
            else
                animator.SetTrigger("StrongAttack");

            SkillDamage = EnemyInfo.StrongAttackDamage;
        }
    }

    public override void Die()
    {
        if(IsDead) return;
        
        base.Die();
        Agent.enabled = false;

        string name = GameDirector.instance.GetObjectName(EnemyInfo.EnemyObject.name);
        DataManager.instance.UpdateMonsterCount(name, -1);
        Player.instance.StatusComponent.GetEXP(name);
    }
}
