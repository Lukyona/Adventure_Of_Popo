using UnityEngine;
using System.Threading.Tasks;


public class PlayerCombatComponent
{
    private Transform playerTransform;

    private LayerMask whatIsEnemy;
    private float sightRange = 13f;
    private float attackRange = 2.5f;
    private float targetRange = 10f; // 타겟팅 가능 범위
    private bool enemyInSightRange, enemyInAttackRange;
    public bool EnemyInTargetRange { get; private set; }

    public GameObject Target { get; set; }
    public int AttackNum { get; private set; }
    private PlayerAttack[] attacks;
    private float[] lastAttackTimes; // 공격을 마지막으로 수행한 시간

    public float SkillDamage { get; private set; }

    public void Start()
    {
        playerTransform = Player.instance.transform;
        whatIsEnemy = LayerMask.GetMask("Enemy");
        attacks = new PlayerAttack[] { new PawAttack(), new TailWhipAttack(), new RollAttack() };
        lastAttackTimes = new float[attacks.Length];
    }

    public void Update()
    {
        HandleAttackInput();

        enemyInSightRange = Physics.CheckSphere(playerTransform.position, sightRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(playerTransform.position, attackRange, whatIsEnemy);
        EnemyInTargetRange = Physics.CheckSphere(playerTransform.position, targetRange, whatIsEnemy);

        if (enemyInSightRange)
        {
            if (GameManager.instance.MainCount == 2 && !GameManager.instance.IsTalking)//소개 후 첫 몬스터 발견
            {
                DialogueManager.instance.SetDialogue(4);//몬스터 첫 발견
                Player.instance.MovementComponent.DontMove();
                GameManager.instance.StartTalk();
            }
        }

        if (!EnemyInTargetRange && Player.instance.GetTarget())//타겟 몬스터가 타겟팅 범위 벗어나면
        {
            Player.instance.SetTarget(null); //타겟 해제
            EnemyHUD.instance.DisappearEnemyInfo();
        }

        if (GameManager.instance.MainCount == 8)
        {
            sightRange = 30f;
            if (enemyInSightRange && !GameManager.instance.IsTalking)
            {
                DialogueManager.instance.SetDialogue(11);
                GameManager.instance.StartTalk();
                CameraController.instance.SetFixedState(false);//카메라 확대축소 가능
            }
        }
    }

    private void HandleAttackInput()
    {
        if (Player.instance.Animator.GetCurrentAnimatorStateInfo(0).IsName("Fox_Idle"))
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    int level = Player.instance.StatusComponent.CurrentLevel;
                    if ((i == 1 && level < 3) || (i == 2 && level < 5)) break;

                    if (Time.time - lastAttackTimes[i] >= attacks[i].Cooldown)
                    {
                        ExecuteAttack(i);
                        lastAttackTimes[i] = Time.time;
                    }
                    AttackNum = i + 1;
                }
            }
        }
    }

    private async void ExecuteAttack(int attackIndex)
    {
        if (Target != null)
        {
            Vector3 targetPos = Target.transform.position;
            targetPos.y = Player.instance.PlayerPos.y;
            Player.instance.transform.LookAt(targetPos);

            await Task.Delay((int)(0.2f + 0.02f * attackIndex)); //여기

            UpdateNPCState();
        }
        SkillDamage = attacks[attackIndex].Damage;
        Player.instance.Animator.SetTrigger($"Attack{attackIndex + 1}");
        attacks[attackIndex].Execute(Target);
    }

    private void UpdateNPCState()
    {
        if (Target.GetComponent<IEnemyController>().IsDead())//타겟 죽었을 때, 문지기/보스 제외
        {
            Player.instance.SetFriendCombatState(false);
        }

        if (enemyInAttackRange)//타겟이 있어야하며 공격범위 안에 있을 때
        {
            if (GameManager.instance.MainCount >= 5)
            {
                Player.instance.SetFriendCombatState(true);
            }
        }
    }

    public void TakeDamage(float damage, GameObject attacker)//플레이어가 입는 데미지
    {
        Player.instance.SetFriendCombatState(true, attacker.transform);
        Player.instance.StatusComponent.ModifyHealth(-damage);
        UIManager.instance.ShowDamageText(Player.instance.gameObject, damage);
    }
}
