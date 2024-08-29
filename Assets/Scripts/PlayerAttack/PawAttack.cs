using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : PlayerAttack
{
    public override void Execute(GameObject target)
    {
        if (target.name.Contains("Turtle"))
        {
            target.GetComponent<EnemyAi>().TakeDamage(12);
        }
        else
        {
            target.GetComponent<EnemyAi>().TakeDamage(15);
        }
        SoundManager.instance.PlayAttack1Sound();
    }

    public override float Cooldown => 0f; // 첫 번째 공격은 쿨타임이 없음
}


/*




public class Attack3 : Attack
{
    
}
*/

/*
public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;

    public GameObject target;
    private Attack[] attacks;
    private float[] attackCooldowns;
    private float[] lastAttackTimes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 공격 배열 초기화
        attacks = new Attack[] { new Attack1(), new Attack2(), new Attack3() };
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
                    if (Time.time - lastAttackTimes[i] >= attacks[i].Cooldown)
                    {
                        ExecuteAttack(i);
                        lastAttackTimes[i] = Time.time;
                    }
                }
            }
        }
    }

    private void ExecuteAttack(int attackIndex)
    {
        if (target != null)
        {
            GameDirector.instance.Player.transform.LookAt(target.transform);
            ThirdPlayerMovement.instance.foxAnimator.SetTrigger($"Attack{attackIndex + 1}");
            Invoke(nameof(GiveDamage), 0.2f + 0.02f * attackIndex);
            attacks[attackIndex].Execute(target);
        }
    }
}
*/