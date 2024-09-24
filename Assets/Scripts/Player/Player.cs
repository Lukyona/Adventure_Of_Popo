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

    BoxCollider attackCollider;

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
        attackCollider = GetComponentInChildren<BoxCollider>();
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
        attackCollider.gameObject.layer = LayerMask.NameToLayer("PlayerAttack");
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        attackCollider.gameObject.layer = LayerMask.NameToLayer("Player");
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("EnemyAttack")) return;

        EnemyCombatComponent eComp = other.GetComponentInParent<IEnemyController>().GetCombatComponent();
        TakeDamage(eComp.SkillDamage, other.gameObject);
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        CombatComponent.TakeDamage(damage, attacker);
    }

    public void SetFriendCombatState(bool value, GameObject attacker = null)
    {
        FriendController[] friends = FindObjectsOfType<FriendController>();

        foreach(FriendController f in friends)
        {
            f.IsInCombat = value;
            if(value != false && CombatComponent.Target == null) f.CombatTarget = attacker.transform;
        }
    }

    public void Die()
    {
        GameDirector.instance.Fox_Cant_Move();//플레이어 이동 금지
        ThirdPlayerMovement.instance.foxAnimator.SetTrigger("Die");//쓰러짐 애니메이션

        if(GetTarget() != null)
        {
            SetTarget(null);
        }
        
        MyTaskManager.instance.ExecuteAfterDelay(UIManager.instance.StartBlackOut, 2f);
        SetFriendCombatState(false);
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
