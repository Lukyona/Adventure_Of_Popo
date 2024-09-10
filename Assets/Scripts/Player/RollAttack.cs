using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollAttack : PlayerAttack
{
    public override void Execute(GameObject target)
    {
        SoundManager.instance.PlayAttack3Sound();
    }

    public override float Cooldown => 5f; // 세 번째 공격은 5초 쿨타임

    public override float Damage => 25f;

}
