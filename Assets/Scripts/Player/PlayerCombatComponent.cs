using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class PlayerCombatComponent
{
    public GameObject Target { get; set;}
    public int AttackNum {get; private set;}
    private PlayerAttack[] attacks;
    private float[] attackCooldowns;
    private float[] lastAttackTimes; // 공격을 마지막으로 수행한 시간

    public float SkillDamage { get; private set;}

    public void Start() 
    {
        attacks = new PlayerAttack[] { new PawAttack(), new TailWhipAttack(), new RollAttack() };
        attackCooldowns = new float[attacks.Length];
        lastAttackTimes = new float[attacks.Length];    
    }

    public void Update()
    {
        if (ThirdPlayerMovement.instance.foxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fox_Idle"))
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    int level = Player.instance.StatusComponent.CurrentLevel;
                    if((i == 1 && level < 3) || (i == 2 && level < 5)) break;
                    
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

    async void ExecuteAttack(int attackIndex)
    {
        if (Target != null)
        {
            //Player.instance.transform.LookAt(Target.transform);
            Vector3 directionToTarget = Target.transform.position - Player.instance.transform.position;
            Player.instance.transform.rotation = Quaternion.Slerp(Player.instance.transform.rotation, Quaternion.LookRotation(directionToTarget), 0.1f);

            await Task.Delay((int)(0.2f + 0.02f * attackIndex)); //여기

            UpdateNPCState();
            SkillDamage = attacks[attackIndex].Damage;
        }
        ThirdPlayerMovement.instance.foxAnimator.SetTrigger($"Attack{attackIndex + 1}");
        attacks[attackIndex].Execute(Target);
    }

    void UpdateNPCState()//타겟에게 데미지 입히는 함수
    {
        if (Target.GetComponent<IEnemyController>().IsDead())//타겟 죽었을 때, 문지기/보스 제외
        {
            Player.instance.SetFriendCombatState(false);
        }
      
        if (ThirdPlayerMovement.instance.monsterInAttackRange)//타겟이 있어야하며 공격범위 안에 있을 때
        {
            if(GameDirector.instance.mainCount >= 5)
            {
                Player.instance.SetFriendCombatState(true);
            }
        } 
    }

    public void TakeDamage(float damage, GameObject attacker)//플레이어가 입는 데미지
    {
        Player.instance.SetFriendCombatState(true, attacker);

        Player.instance.StatusComponent.ModifyHealth(-damage);
        UIManager.instance.ShowPlayerDamageText(damage);
    }
}
