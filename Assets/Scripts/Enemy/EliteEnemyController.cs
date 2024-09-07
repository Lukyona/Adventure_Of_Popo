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

        combatComponent = new EliteEnemyCombatacomponent
        {
            EnemyInfo = enemyInfo,
            OwnerController = this,
            Fireball = fireball
        };
 
        combatComponent.Start();
    }

    void Update()
    {
        combatComponent.Update();
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

    public void Disable()
    {
        enabled = false;
    }
}
