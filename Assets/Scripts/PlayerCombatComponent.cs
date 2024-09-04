using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatComponent : CombatComponent
{
    public GameObject target;
    private PlayerAttack[] attacks;
    private float[] attackCooldowns;
    private float[] lastAttackTimes; // 공격을 마지막으로 수행한 시간

    private void Start() 
    {
        attacks = new PlayerAttack[] { new PawAttack(), new TailWhipAttack(), new RollAttack() };
        attackCooldowns = new float[attacks.Length];
        lastAttackTimes = new float[attacks.Length];    
    }

    private void Update()
    {
        if (ThirdPlayerMovement.instance.foxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fox_Idle"))
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    if((i == 1 && PlayerInfoManager.instance.level < 3) || (i == 2 && PlayerInfoManager.instance.level < 5)) break;
                    
                    if (Time.time - lastAttackTimes[i] >= attacks[i].Cooldown)
                    {
                        ExecuteAttack(i);
                        lastAttackTimes[i] = Time.time;
                    }
                }
            }
        }
    }

    void ExecuteAttack(int attackIndex)
    {
        if (target != null)
        {
            GameDirector.instance.Player.transform.LookAt(target.transform);
            Invoke(nameof(GiveDamage), 0.2f + 0.02f * attackIndex);
            attacks[attackIndex].Execute(target);
        }
        ThirdPlayerMovement.instance.foxAnimator.SetTrigger($"Attack{attackIndex + 1}");
    }

    void GiveDamage()//타겟에게 데미지 입히는 함수
    {
        if (GameDirector.instance.mainCount < 9 && target.GetComponent<EnemyAi>().health <= 0)//타겟 죽었을 때, 문지기/보스 제외
        {
            GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
            GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
        }
        if(GameDirector.instance.mainCount >= 9 && target.GetComponent<MonsterController>().health <= 0)//문지기/보스 죽었을 때
        {
            GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
            GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
        }

        if (ThirdPlayerMovement.instance.monsterInAttackRange)//타겟이 있어야하며 공격범위 안에 있을 때
        {
            if(GameDirector.instance.mainCount >= 5)
            {
                GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = true;//동료 전투 상태 돌입
                GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = true;//동료 전투 상태 돌입
            }
        } 
    }
}
