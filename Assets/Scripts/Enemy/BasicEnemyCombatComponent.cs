using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;


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
        base.AttackPlayer();

        Agent.SetDestination(EnemyInfo.Transform.position); // 정지

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

        if(PlayerInfoManager.instance.death && (EnemyInfo.EnemyObject.name.Contains("Slime") || EnemyInfo.EnemyObject.name.Contains("Turtle")))
        {
            animator.SetTrigger("Victory");
        }
    }

    public override void Die()
    {
        base.Die();
        Agent.enabled = false;
        PlayerInfoManager.instance.UpdateMonsterCount(GameDirector.instance.GetObjectName(EnemyInfo.EnemyObject.name), -1);
        PlayerInfoManager.instance.Invoke(nameof(PlayerInfoManager.instance.GetEXP), 1.5f);
    }
}
