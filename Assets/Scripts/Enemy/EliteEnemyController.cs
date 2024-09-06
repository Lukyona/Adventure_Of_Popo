using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyController : MonoBehaviour
{
    [SerializeField] EnemyInfo enemyInfo;
    EnemyCombatComponent combatComponent;
    [SerializeField] GameObject fireball;

    void Start()
    {
        combatComponent = new EliteEnemyCombatacomponent
        {
            OwnerController = this,
            EnemyInfo = enemyInfo,
            Fireball = fireball
        };
    }
}
