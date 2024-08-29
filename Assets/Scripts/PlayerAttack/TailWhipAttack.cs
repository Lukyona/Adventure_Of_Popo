using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailWhipAttack : PlayerAttack
{
    public override void Execute(GameObject target)
    {
        target.GetComponent<EnemyAi>().TakeDamage(20);
        SoundManager.instance.PlayAttack2Sound();
    }

    public override float Cooldown => 2f; // 두 번째 공격은 2초 쿨타임
}
