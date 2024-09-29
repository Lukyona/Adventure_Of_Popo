using UnityEngine;

public class TailWhipAttack : PlayerAttack
{
    public override void Execute(GameObject target)
    {
    }

    public override float Cooldown => 2f; // 두 번째 공격은 2초 쿨타임

    public override float Damage => 20f;

}
