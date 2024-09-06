using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyController : MonoBehaviour, IEnemyController
{
    [SerializeField] EnemyInfo enemyInfo;
    EnemyCombatComponent combatComponent;
    [SerializeField] GameObject fireball;

    public void Start()
    {
        enemyInfo.EnemyObject = gameObject;
        enemyInfo.EnemyTransform = transform;

        combatComponent = new EliteEnemyCombatacomponent
        {
            OwnerController = this,
            EnemyInfo = enemyInfo,
            Fireball = fireball
        };
    }

    public void TakeDamage(int damage)
    {
        combatComponent.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return combatComponent.IsDead;
    }

    public float GetMaxHealth()
    {
        return enemyInfo.MaxHealth;
    }
}
