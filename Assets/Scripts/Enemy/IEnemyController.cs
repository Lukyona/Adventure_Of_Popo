using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyController
{
    void Start();

    int GetLevel();

    float GetMaxHealth();

    void TakeDamage(float damage);

    bool IsDead();

    void Disable();

    void DestroyMyself();

}
