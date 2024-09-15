using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Vector3 PlayerPos {get { return transform.position; }
                              set { transform.position = value; }}

    public Quaternion PlayerRot {get { return transform.rotation; }
                              set { transform.rotation = value; }}
    public PlayerCombatComponent CombatComponent {get; private set;}
    public PlayerStatusComponent StatusComponent {get; private set;}


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        StatusComponent = new PlayerStatusComponent();
        CombatComponent = new PlayerCombatComponent();
    }

    void Start()
    {
        CombatComponent.Start();
    }

    void Update()
    {
        StatusComponent.Update();
        CombatComponent.Update();
    }

    public void SetTarget(GameObject target)
    {
        CombatComponent.Target = target;
    }

    public GameObject GetTarget()
    {
        return CombatComponent.Target;
    }

    public void EnableAttackCollider()
    {
        tag = "PlayerAttack";
        GetComponent<BoxCollider>().enabled = true;
    }

    public void DisableAttackCollider()
    {
        tag = "Player";

        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("EnemyAttack")) return;

        EnemyCombatComponent eComp = other.GetComponent<IEnemyController>().GetCombatComponent();
        TakeDamage(eComp.SkillDamage);
    }

    public void TakeDamage(float damage)
    {
        CombatComponent.TakeDamage(damage);
    }

    public bool IsDead()
    {
        return StatusComponent.CurrentHealth > 0 ? false : true;
    }

    public void Revive()
    {
        ThirdPlayerMovement.instance.foxAnimator.SetTrigger("StandUp");//여우 일어나기
        GameDirector.instance.Invoke(nameof(GameDirector.instance.Fox_Can_Move), 1f);//이동 가능
        UIManager.instance.DeactiveBlackScreen();
        CameraController.instance.SetFixedState(false); //카메라가 플레이어 위치로 이동
    }
}
