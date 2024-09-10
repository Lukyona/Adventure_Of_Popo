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
            Fireball = fireball
        };
 
        combatComponent.Start();
    }

    void Update()
    {
        combatComponent.Update();
    }

    public float GetMaxHealth()
    {
        return enemyInfo.MaxHealth;
    }    
    
    public int GetLevel()
    {
        return enemyInfo.Level;
    }

    public void TakeDamage(float damage)
    {
        combatComponent.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return combatComponent.IsDead;
    }

    public void Disable()
    {
        enabled = false;
    }

    public void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
