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
    }

    void Update()
    {
        
    }

    public void SetTarget(GameObject target)
    {
        combatComponent.Target = target;
    }

    public GameObject GetTarget()
    {
        return combatComponent.Target;
    }
}
