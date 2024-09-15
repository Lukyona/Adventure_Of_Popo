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
    PlayerCombatComponent combatComponent;
    public PlayerStatusComponent StatusComponent {get; private set;}


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        StatusComponent = new PlayerStatusComponent();
        combatComponent = new PlayerCombatComponent();
    }

    void Start()
    {
        combatComponent.Start();
    }

    void Update()
    {
        StatusComponent.Update();
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

        SoundManager.instance.PlayAttackSound(combatComponent.AttackNum);
        other.gameObject.GetComponent<IEnemyController>().TakeDamage(combatComponent.SkillDamage);
    }

    public void TakeDamage(float damage)
    {
        combatComponent.TakeDamage(damage);
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
