using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;

    public GameObject target;
    public bool alreadyAttacked1 = false; //공격 텀을 위한 변수
    public bool alreadyAttacked2 = false;
    public bool alreadyAttacked3 = false;
    public LayerMask whatIsFence;//부서질 펜스
    int fence_hit = 0;//펜스 공격한 횟수

    public int attackNum = 0;// 몇번 공격인지 구분
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (ThirdPlayerMovement.instance.foxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fox_Idle"))//현재 애니메이션이 idle이면
        {
            if(!alreadyAttacked1)//공격 텀이 리셋되었을 때만
            {              
                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))//숫자키 1을 눌렀을 때
                {
                    if(target != null)
                    {
                        GameDirector.instance.Player.transform.LookAt(target.transform);//공격대상 쳐다보기
                    }
                    alreadyAttacked1 = true;
                    ThirdPlayerMovement.instance.foxAnimator.SetTrigger("Attack1");//발공격 애니메이션 실행
                    Invoke(nameof(GiveDamage), 0.2f);
                    attackNum = 1;

                    if(Physics.CheckSphere(GameDirector.instance.Player.transform.position, 5f, whatIsFence) && GameDirector.instance.mainCount == 5)//펜스가 범위내에 있을 때
                    {
                        fence_hit++;
                        SoundManager.instance.PlayAttack1Sound();
                        if (fence_hit == 5)//공격횟수가 5일 때 한번만 발생
                        {
                            GameDirector.instance.Invoke(nameof(GameDirector.instance.Destroy_Fence), 0.3f);
                            GameDirector.instance.HitWoodenFence();
                        }
                    }
                }
            }
            if (!alreadyAttacked2)
            {
                if (PlayerInfoManager.instance.level >= 3 && (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)))//레벨 3이상, 숫자키 2을 눌렀을 때
                {
                    if (target != null)
                    {
                        GameDirector.instance.Player.transform.LookAt(target.transform);//공격대상 쳐다보기
                    }
                    alreadyAttacked2 = true;
                    ThirdPlayerMovement.instance.foxAnimator.SetTrigger("Attack2");//꼬리공격 애니메이션 실행
                    Invoke(nameof(GiveDamage), 0.24f);
                    attackNum = 2;
                }
            }
            if (!alreadyAttacked3)
            {
                if (PlayerInfoManager.instance.level >= 5 && (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)))//레벨 5이상, 숫자키 3을 눌렀을 때
                {
                    if (target != null)
                    {
                        GameDirector.instance.Player.transform.LookAt(target.transform);//공격대상 쳐다보기
                    }
                    alreadyAttacked3 = true;
                    ThirdPlayerMovement.instance.foxAnimator.SetTrigger("Attack3");//구르기 공격 애니메이션 실행
                    Invoke(nameof(GiveDamage), 0.3f);
                    attackNum = 3;
                }
            }
        }
    }

    void GiveDamage()//타겟에게 데미지 입히는 함수
    {
        if (GameDirector.instance.mainCount < 9 && target != null && target.GetComponent<EnemyAi>().health <= 0)//타겟 죽었을 때, 문지기/보스 제외
        {
            GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
            GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
        }
        if(GameDirector.instance.mainCount >= 9 && target != null && target.GetComponent<MonsterController>().health <= 0)//문지기/보스 죽었을 때
        {
            GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
            GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = false;//동료 전투 상태 해제
        }

        if (target != null && ThirdPlayerMovement.instance.monsterInAttackRange)//타겟이 있어야하며 공격범위 안에 있을 때
        {
            if(GameDirector.instance.mainCount >= 5)
            {
                GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = true;//동료 전투 상태 돌입
                GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = true;//동료 전투 상태 돌입
            }
            switch (attackNum)//공격 종류에 따라 다른 데미지
            {
                case 1://1번 공격, 기본데미지 15
                    if (target.name.Contains("Turtle"))//가시거북이면
                    { 
                        target.GetComponent<EnemyAi>().TakeDamage(12);//12데미지
                    }
                    else//거북이 제외 몬스터
                    {
                        if(GameDirector.instance.mainCount < 9)//보스/문지기 제외
                        {
                            target.GetComponent<EnemyAi>().TakeDamage(15);//15데미지
                        }
                        else
                        {
                            target.GetComponent<MonsterController>().TakeDamage(15);//15데미지
                        }
                    }
                    SoundManager.instance.PlayAttack1Sound();
                    break;
                case 2: //데미지 20
                    if(GameDirector.instance.mainCount < 9)
                    {
                        target.GetComponent<EnemyAi>().TakeDamage(20);
                    }
                    else
                    {
                        target.GetComponent<MonsterController>().TakeDamage(20);//20데미지
                    }
                    Invoke(nameof(Can_Attack2), 2f);//2초 쿨타임
                    SoundManager.instance.PlayAttack2Sound();
                    break;
                case 3://데미지 25
                    target.GetComponent<MonsterController>().TakeDamage(25);
                    Invoke(nameof(Can_Attack3), 5f);//5초 쿨타임
                    SoundManager.instance.PlayAttack3Sound();
                    break;
            }
            if (alreadyAttacked1)
            {
                alreadyAttacked1 = false;
            }
        }
        else
        {
            switch(attackNum)
            {
                case 1:
                    alreadyAttacked1 = false;
                    break;
                case 2:
                    Invoke(nameof(Can_Attack2), 2f);//2초 쿨타임
                    break;
                case 3:
                    Invoke(nameof(Can_Attack3), 5f);//5초 쿨타임
                    break;
            }
        }      
    }

    void Can_Attack2()
    {
        alreadyAttacked2 = false;
    }

    void Can_Attack3()
    {
        alreadyAttacked3 = false;
    }
}
