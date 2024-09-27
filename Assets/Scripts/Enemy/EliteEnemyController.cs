using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyController : MonoBehaviour, IEnemyController
{
    public Animator Animator {get; private set;}

    BoxCollider AttackCollider {get; set;}

    [SerializeField] EnemyInfo enemyInfo;
    EnemyCombatComponent combatComponent;
    [SerializeField] GameObject fireball;

    public void Start()
    {
        Animator = GetComponent<Animator>();
        AttackCollider = GetComponentInChildren<BoxCollider>();

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

    public EnemyCombatComponent GetCombatComponent()
    {
        return combatComponent;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(IsDead()) return;

        if(other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            PlayerCombatComponent pComp = Player.instance.CombatComponent;
            SoundManager.instance.PlayAttackSound(pComp.AttackNum);
            TakeDamage(pComp.SkillDamage);
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("NpcAttack"))
        {
            float damage = other.GetComponentInParent<FriendController>().SkillDamage;
            TakeDamage(damage);
        }
        other.GetComponent<BoxCollider>().enabled = false; // 중복 감지 방지
    }

    public void EnableAttackCollider()
    {
        AttackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        AttackCollider.enabled = false;
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
