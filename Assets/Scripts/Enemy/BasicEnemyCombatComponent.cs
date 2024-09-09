using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
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

    public async override void AttackPlayer()
    {
        if(!canAttack) return;

        if(EnemyInfo.EnemyObject.name.Contains("Slime") || EnemyInfo.EnemyObject.name.Contains("Turtle"))
        {
            animator.SetBool("Battle", true);

            if(PlayerInfoManager.instance.death)
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
            PlayerInfoManager.instance.PlayerDamage(EnemyInfo.NormalAttackDamage);
        }
        else
        {
            if(EnemyInfo.EnemyObject.name.Contains("Bat") || EnemyInfo.EnemyObject.name.Contains("Mushroom"))
                animator.SetTrigger("Attack");
            else
                animator.SetTrigger("StrongAttack");

            PlayerInfoManager.instance.PlayerDamage(EnemyInfo.StrongAttackDamage);
        }

        await Task.Delay((int)EnemyInfo.TimeBetweenAttacks * 1000); // TimeBetweenAttacks초만큼 기다리고 다음 실행

        ResetAttack();
    }

    public override void Die()
    {
        if(IsDead) return;

        base.Die();
        Agent.enabled = false;
        PlayerInfoManager.instance.UpdateMonsterCount(GameDirector.instance.GetObjectName(EnemyInfo.EnemyObject.name), -1);
        PlayerInfoManager.instance.GetEXP(GameDirector.instance.GetObjectName(EnemyInfo.EnemyObject.name));
    }
}
