using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Animator Animator { get; private set; }

    public Vector3 PlayerPos
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Quaternion PlayerRot
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }
    public PlayerMovementComponent MovementComponent { get; private set; }
    public PlayerCombatComponent CombatComponent { get; private set; }
    public PlayerStatusComponent StatusComponent { get; private set; }

    private BoxCollider attackCollider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        MovementComponent = new PlayerMovementComponent();
        StatusComponent = new PlayerStatusComponent();
        CombatComponent = new PlayerCombatComponent();
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MovementComponent.Start();
        CombatComponent.Start();
        attackCollider = GetComponentInChildren<BoxCollider>();
    }

    private void Update()
    {
        MovementComponent.Update();
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

    public void EnableMovement()
    {
        MovementComponent.Enabled = true;//플레이어 이동 가능
        CameraController.instance.SetFixedState(false);//카메라 확대축소 가능
        GetComponent<CharacterController>().enabled = true;//캐릭터 컨트롤러 켜기
    }

    public void DisableMovement()
    {
        MovementComponent.Enabled = false;//플레이어 이동 불가
        CameraController.instance.SetFixedState(true); //카메라 회전 불가
        GetComponent<CharacterController>().enabled = false;//캐릭터 컨트롤러 끄기
        MovementComponent.DontMove();
    }

    public void EnableAttackCollider()
    {
        if (GameManager.instance.MainCount >= 9)
        {
            attackCollider.center = new Vector3(0, 0.7f, 2);
            attackCollider.size = new Vector3(1, 1.2f, 2);
        }
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("EnemyAttack")) return;

        other.GetComponent<BoxCollider>().enabled = false; // 중복 감지 방지
        EnemyCombatComponent eComp = other.GetComponentInParent<IEnemyController>().GetCombatComponent();
        TakeDamage(eComp.SkillDamage, other.gameObject);
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        CombatComponent.TakeDamage(damage, attacker);
    }

    public void SetFriendCombatState(bool value, Transform attacker = null)
    {
        FriendController[] friends = FindObjectsOfType<FriendController>();

        foreach (FriendController f in friends)
        {
            if (value != false && attacker != null)
                f.CombatTarget = attacker.parent.transform;

            f.IsInCombat = value;
        }
    }

    public void Die()
    {
        DisableAttackCollider();
        DisableMovement();//플레이어 이동 금지
        Animator.SetTrigger("Die");//쓰러짐 애니메이션

        if (GetTarget() != null)
        {
            SetTarget(null);
            EnemyHUD.instance.DisappearEnemyInfo();
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
        Animator.SetTrigger("StandUp");//여우 일어나기
        Invoke(nameof(EnableMovement), 1f);//이동 가능
        UIManager.instance.DeactiveBlackScreen();
        CameraController.instance.SetFixedState(false); //카메라가 플레이어 위치로 이동
        StatusComponent.CurrentHealth = 10;
        UIManager.instance.UpdatePlayerHealthUI();
    }
}
