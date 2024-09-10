using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    PlayerCombatComponent combatComponent;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        combatComponent = new PlayerCombatComponent();
        combatComponent.Start();
    }

    void Update()
    {
        combatComponent.Update();
    }

    public void SetTarget(GameObject target)
    {
        combatComponent.Target = target;
    }

    public GameObject GetTarget()
    {
        return combatComponent.Target;
    }

    public void ActivateCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    public void DeactivateCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy")) return;

        other.gameObject.GetComponent<IEnemyController>().TakeDamage(combatComponent.SkillDamage);
    }
}
