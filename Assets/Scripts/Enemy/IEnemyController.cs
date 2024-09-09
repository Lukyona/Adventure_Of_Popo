using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyController
{
    void Start();

    void TakeDamage(int damage);

    bool IsDead();

    float GetMaxHealth();

    void Disable();

    void DestroyMyself();

}
