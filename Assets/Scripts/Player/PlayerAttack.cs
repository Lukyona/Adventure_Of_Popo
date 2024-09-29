using UnityEngine;

public abstract class PlayerAttack
{
    public abstract void Execute(GameObject target);
    public abstract float Cooldown { get; }

    public abstract float Damage { get; }
}
