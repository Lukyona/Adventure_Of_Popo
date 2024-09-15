using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyController
{
    Animator Animator {get;}

    void Start();

    int GetLevel();

    float GetMaxHealth();

    EnemyCombatComponent GetCombatComponent();

    void TakeDamage(float damage);

    bool IsDead();

    void Disable();

    void DestroyMyself();

    void OnTriggerEnter(Collider other);

    void EnableAttackCollider();
    void DisableAttackCollider();

}
