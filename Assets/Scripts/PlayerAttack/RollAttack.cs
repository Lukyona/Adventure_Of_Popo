using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollAttack : PlayerAttack
{
    public override void Execute(GameObject target)
    {
        target.GetComponent<MonsterController>().TakeDamage(25);
        SoundManager.instance.PlayAttack3Sound();
    }

    public override float Cooldown => 5f; // 세 번째 공격은 5초 쿨타임
}
