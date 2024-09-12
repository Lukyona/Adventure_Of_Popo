using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyController : MonoBehaviour, IEnemyController
{
    public Animator Animator {get; private set;}

    [SerializeField] EnemyInfo enemyInfo;
    EnemyCombatComponent combatComponent;
    [SerializeField] GameObject fireball;

    public void Start()
    {
        Animator = GetComponent<Animator>();

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

    public void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        other.gameObject.GetComponent<Player>().TakeDamage(combatComponent.SkillDamage);
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
