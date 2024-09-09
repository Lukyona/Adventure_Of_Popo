using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAttack
{
    public abstract void Execute(GameObject target);
    public abstract float Cooldown { get; }
}
