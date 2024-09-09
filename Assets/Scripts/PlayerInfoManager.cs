using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager instance;

    public GameObject BlackScreen;//플레이어 죽음 시 블랙스크린
    public Animator blackAnimator;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        blackAnimator = BlackScreen.GetComponent<Animator>();       
        LoadPlayerData();//저장된 데이터 로드
    }

    public void ResetData()//데이터 초기화
    {
        PlayerPrefs.DeleteAll();//저장된 것 전부 삭제
        Application.Quit();//게임 종료
    }

    public void HPCheat()
    {
        if(hp >= 1)
        {
            hp += 100;
            if (hp >= hpMax)
            {
                hp = hpMax;
            }
            HP_Update();
        }
    }

    public int sp = 12; // 스태미나 포인트, 초기값 12
    int spMax = 12; //스태미나 최대값, 초기값 12
    public Text spText; //스태미나 텍스트
    public Text spMaxText; //스태미나 최대치 텍스트
    public Image spBar;//스태미나 바
    bool useSp = false;//스태미나 소모 함수를 위한 변수

    public void ConsumeSP() //스태미나 소모
    {
        if (!useSp)
        {
            useSp = true;//스태미나 소모 중
            sp -= 1;
            if (sp <= 0)
            {
                sp = 0;
            }
            SP_Update();
            
            switch (level)
            {
                case 1:
                    Invoke(nameof(ConsumeSP2), 0.07f);
                    break;
                case 2://2렙부터 알아서 테스트하면서 고치기,공식으로 맞지않음
                    Invoke(nameof(ConsumeSP2), 0.166667f);
                    break;
                case 3:
                    Invoke(nameof(ConsumeSP2), 0.166667f);
                    break;
                case 4:
                    Invoke(nameof(ConsumeSP2), 0.166667f);
                    break;
                case 5:
                    Invoke(nameof(ConsumeSP2), 0.166667f);
                    break;
            }
        }
        
    }

    void ConsumeSP2()
    {
        useSp = false;
    }

    bool recSp = false; //스태미나 회복중

    private void Update()
    {
        if(sp < spMax && !recSp && !ThirdPlayerMovement.instance.running && !death)
        {//현재 스태미나가 스태미나 최대치 미만이고 스태미나 회복 중이 아니고 달리지 않을 때
            recSp = true;
            Invoke(nameof(RecoverSP), 0.3f);
        }
        
        if(hp < hpMax && !recHp && !death)//죽음 상태가 아닐 때
        {
            recHp = true;
            Invoke(nameof(RecoverHP), 1f);
        }
    }

    void RecoverSP()//스태미나 회복
    {
        if(sp == spMax)
        {
            recSp = false;
        }
        else
        {
            sp += 1;
            SP_Update();

            switch(level)//렙마다 회복속도 차이남
            {
                case 1:
                    Invoke(nameof(RecoverSP), 1f);
                    break;
                case 2:
                    Invoke(nameof(RecoverSP), 0.75f);
                    break;
                case 3:
                    Invoke(nameof(RecoverSP), 0.5f);
                    break;
                case 4:
                    Invoke(nameof(RecoverSP), 0.4f);
                    break;
                case 5:
                    Invoke(nameof(RecoverSP), 0.3f);
                    break;
            }
        }
    }

    void SP_Update()//스태미나 갱신, 레벨에 따라 게이지 차는 양 달라짐
    {
        spText.text = sp.ToString(); //화면상의 생명력 텍스트 변경
        spBar.fillAmount = 0;
        switch (level)
        {
            case 1://1렙일 때, 스태미나 한도 12
                for (int i = 0; i < sp; i++)
                {
                    spBar.fillAmount += 0.083333f;
                }
                break;
            case 2://2렙, 스태미나 한도 14
                for (int i = 0; i < sp; i++)
                {
                    spBar.fillAmount += 0.07142857f;
                }
                break;
            case 3://3렙, 스태미나 한도 16
                for (int i = 0; i < sp; i++)
                {
                    spBar.fillAmount += 0.0625f;
                }
                break;
            case 4://4렙, 스태미나 한도 18
                for (int i = 0; i < sp; i++)
                {
                    spBar.fillAmount += 0.055556f;
                }
                break;
            case 5://5렙, 스태미나 한도 20
                for (int i = 0; i < sp; i++)
                {
                    spBar.fillAmount += 0.05f;
                }
                break;
        }
    }

    int hp = 100; //현재 생명력
    int hpMax = 100; //생명력 초기값 100
    public Text hpText;
    public Image hpBar;
    public Text hpMaxText; //생명력 최대치 텍스트
    public bool death = false;//플레이어의 죽음 판별

    [SerializeField] GameObject damageText = null;//플레이어 데미지 프리팹
    public Canvas canvas;
    //public int a = 0; //공격 종류
    public void PlayerDamage(int damage)//플레이어가 입는 데미지
    {
        if(GameDirector.instance.mainCount == 10)//보스전
        {
            GameDirector.instance.friend_slime.GetComponent<FriendController>().battle = true;//동료 전투 상태 돌입
            GameDirector.instance.friend_mushroom.GetComponent<FriendController>().battle = true;//동료 전투 상태 돌입
        }
        
        hp -= damage;
        hpBar.fillAmount -= damage / hpMax;

        GameObject dText = Instantiate(damageText, canvas.transform.position + new Vector3(-250f, 20f, 0), Quaternion.identity, canvas.transform);
        dText.GetComponent<DamageText>().SetDamage(damage);

        if (hp <= 0)//생명력이 0이하일 땐
        {
            hp = 0;
            GameDirector.instance.Fox_Cant_Move();//플레이어 이동 금지
            ThirdPlayerMovement.instance.foxAnimator.SetTrigger("Die");//쓰러짐 애니메이션
            death = true;
            Invoke(nameof(BlackOut_On), 2f);//검은 화면으로 전환
            if(Player.instance.GetTarget() != null)
            {
                Player.instance.SetTarget(null);
            }
        }
        HP_Update();
    }//몬스터의 공격에 데미지 입음

    public bool recHp = false;
    void RecoverHP()//생명력 회복
    {
        if(!death)//죽음 아닐 때, 혹은 죽음 상태 풀리고 나서 회복
        {
            if (hp == hpMax)
            {
                recHp = false;
            }
            else
            {
                hp += 1;
                HP_Update();

                switch (level)//렙마다 회복속도 차이남
                {
                    case 1:
                        Invoke(nameof(RecoverHP), 2f);
                        if (!ThirdPlayerMovement.instance.monsterInTargetRange && GameDirector.instance.mainCount == 3 && !GameDirector.instance.talking)//생명력 깎인 뒤 과일에 대한 설명
                        {
                            DialogueController.instance.SetDialogue(5);
                            GameDirector.instance.Fox_Cant_Move(); //플레이어 이동, 카메라 회전 금지
                            ThirdPlayerMovement.instance.DontMove();
                            GameDirector.instance.Start_Talk();
                        }
                        break;
                    case 2:
                        Invoke(nameof(RecoverHP), 1.8f);
                        break;
                    case 3:
                        Invoke(nameof(RecoverHP), 1.5f);
                        break;
                    case 4:
                        Invoke(nameof(RecoverHP), 1.3f);
                        break;
                    case 5:
                        Invoke(nameof(RecoverHP), 1f);
                        break;
                }
            }
        }
        
    }

    void HP_Update()//생명력 갱신, 레벨에 따라 생명력바 게이지 차는 양 달라짐
    {
        hpText.text = hp.ToString(); //화면상의 생명력 텍스트 변경
        hpBar.fillAmount = 0;
        switch (level)
        {
            case 1://1렙일 때, 체력한도 100
                for (int i = 0; i < hp; i++)
                {
                    hpBar.fillAmount += 0.01f;
                }
                break;
            case 2://2렙, 체력한도 150
                for (int i = 0; i < hp; i++)
                {
                    hpBar.fillAmount += 0.0066667f;
                }
                break;
            case 3://3렙, 체력한도 200
                for (int i = 0; i < hp; i++)
                {
                    hpBar.fillAmount += 0.005f;
                }
                break;
            case 4://4렙, 체력한도 250
                for (int i = 0; i < hp; i++)
                {
                    hpBar.fillAmount += 0.004f;
                }
                break;
            case 5://5렙, 체력한도 300
                for (int i = 0; i < hp; i++)
                {
                    hpBar.fillAmount += 0.0033333f;
                }
                break;
        }
    }

    void BlackOut_On()//플레이어 죽음 시 검은 화면 전환
    {
        BlackScreen.SetActive(true);
        blackAnimator.SetTrigger("BlackOn");
        DialogueController.instance.SetDialogue(1);
        GameDirector.instance.Invoke(nameof(GameDirector.instance.Start_Talk), 3.5f);
        recHp = false;
        recSp = false;
        if (GameDirector.instance.mainCount >= 5)
        {
            FriendController.instance.battle = false;//동료 전투 중지
        }
    }

    public void BlackOut_Off() // 부활, 화면 다시 밝아지고 플레이어 처음 위치로 이동, 능력치 변화
    {
        CameraController.instance.SetFieldOfView(35f);
        CameraController.instance.SetYAxisValue(0.6f);
        CameraController.instance.SetXAxisValue(0f);

        GameDirector.instance.Player.transform.position = new Vector3(280, 0, 80);//플레이어 위치 처음으로
        GameDirector.instance.Player.transform.rotation = Quaternion.Euler(0, 180, 0);//플레이어 회전, 앞을 보도록
        if (GameDirector.instance.mainCount >= 5)
        {
            GameDirector.instance.friend_slime.transform.position = GameDirector.instance.Player.transform.position + new Vector3(2f, 0, -3f);//슬라임동료 위치 조정
        }
        if (GameDirector.instance.mainCount >= 8)
        {
            GameDirector.instance.friend_mushroom.transform.position = GameDirector.instance.Player.transform.position + new Vector3(-2f, 0, -3f);//슬라임동료 위치 조정
        }

        hp = 10;//생명력 10으로 회복
        HP_Update();
        blackAnimator.SetTrigger("BlackOff");
        Invoke(nameof(InActiveBlackScreen), 2.5f);
        CameraController.instance.SetFixedState(false); //카메라가 플레이어 위치로 이동
    }

    void InActiveBlackScreen()//검은화면 비활성화, 플레이어 부활
    {
        ThirdPlayerMovement.instance.foxAnimator.SetTrigger("StandUp");//여우 일어나기
        GameDirector.instance.Invoke(nameof(GameDirector.instance.Fox_Can_Move), 1f);//이동 가능
        BlackScreen.SetActive(false);
        death = false;
    }

    public int exp = 0; //경험치 수치
    int expMax = 50;
    public Text expText;
    public Text expMaxText; //경험치 최대치 텍스트
    public Image expBar;
    public Text middle;//슬래시 텍스트
    public Text max;//5렙일 때 경험치란에 max 표시

    public void GetEXP(string enemyName)//경험치 얻는 함수
    {
        switch(enemyName)
        {
            case "Slime":
                exp += 5;
                break;
            case "Turtle":
                exp += 10;
                break;
            case "Log":
                exp += 20;
                break;
            case "Bat":
                exp += 25;
                break;
            case "Mushroom":
                exp += 33;
                break;
            case "DogKnight":
                exp += 48;
                break;
        }

        if(GameDirector.instance.mainCount == 7)
        {
            Invoke(nameof(Check_All_Died), 1f);
        }

        if (exp >= expMax && level != 5)//레벨업, 5레벨 아닐 때만 가능
        {
            LevelUp();
            levelBoard_Animator.SetTrigger("Down");
        }
        else
        {
            EXP_Update();
        }

        Player.instance.SetTarget(null);
    }

    void EXP_Update()//경험치 갱신, 레벨에 따라 게이지 차는 양 달라짐
    {
        expText.text = exp.ToString(); //화면상의 생명력 텍스트 변경
        if(level != 5)
        {
            expBar.fillAmount = 0;
        }
        switch (level)
        {
            case 1://1렙일 때, 한도 50
                for (int i = 0; i < exp; i++)
                {
                    expBar.fillAmount += 0.02f;
                }
                break;
            case 2://2렙, 한도 80
                for (int i = 0; i < exp; i++)
                {
                    expBar.fillAmount += 0.0125f;
                }
                break;
            case 3://3렙, 한도 120
                for (int i = 0; i < exp; i++)
                {
                    expBar.fillAmount += 0.0083333f;
                }
                break;
            case 4://4렙, 한도 160
                for (int i = 0; i < exp; i++)
                {
                    expBar.fillAmount += 0.0055556f;
                }
                break;
        }        
    }

    public Animator levelBoard_Animator;//레벨업 보드 애니메이터
    public Text levelText2; //레벨업 보드의 레벨 텍스트
    public Text whatCanDo; //현재 레벨에서 무엇을 새로이 할 수 있는지 안내해주는 메세지

    void LevelUp()
    {
        SoundManager.instance.PlayLevelUpSound();
        
        exp = exp - expMax; //현재 경험치 
        EXP_Update();
        level++;
        Level_Update(); //능력치 수치 변경
        switch(level)
        {
            case 2://2렙에서 울타리 표시
                if(GameDirector.instance.mainCount == 4)
                {
                    MonsterHPBar.instance.Fence_arrow();
                }
                break;
            case 3:
                whatCanDo.text = "새로운 공격 기술을 얻었습니다!";
                break;
            case 4:
                whatCanDo.text = "";
                break;
            case 5:
                whatCanDo.text = "새로운 공격 기술을 얻었습니다!";
                break;
        }
        hp = hpMax;//체력과 스태미나 풀로 채워짐
        sp = spMax;
        HP_Update();
        SP_Update();
        levelText2.text = level.ToString();
    }

    public int level = 1; //레벨, 초기값 1
    public Text levelText;//레벨 텍스트

    void Level_Update()
    {
        levelText.text = level.ToString();
        switch (level)//1레벨이 초기값이므로 2레벨부터 정보 갱신
        {
            case 2:
                hpMax = 150;
                spMax = 14;
                expMax = 80;
                break;
            case 3:
                hpMax = 200;
                spMax = 16;
                expMax = 120;
                break;
            case 4:
                hpMax = 250;
                spMax = 18;
                expMax = 160;
                break;
            case 5:
                hpMax = 300;
                spMax = 20;
                middle.gameObject.SetActive(false);//지금까지 표시된 경험치 텍스트들 모두 비활성화
                expText.gameObject.SetActive(false);
                expMaxText.gameObject.SetActive(false);
                max.gameObject.SetActive(true);//max 텍스트 활성화
                expBar.fillAmount = 0;
                expBar.fillAmount += 1f;
                break;
        }

        hpMaxText.text = hpMax.ToString();
        spMaxText.text = spMax.ToString();
        if(level != 5)
        {
            expMaxText.text = expMax.ToString();
        }
    }

    public void Close_LevelUpBoard()
    {
        SoundManager.instance.PlayClickSound();
        levelBoard_Animator.SetTrigger("Up");
        if(level == 3)//3렙, 새로운 기술 안내
        {
            DialogueController.instance.SetDialogue(9);
            GameDirector.instance.Start_Talk();
        }
        if (level == 5)//5렙, 새로운 기술 안내
        {
            DialogueController.instance.SetDialogue(12);
            GameDirector.instance.Start_Talk();
        }
    }

    public void Eat_Food(int f)
    {
        SoundManager.instance.PlayClickSound();
        switch (f)
        {
            case 1://토마토
                hp += 40;
                break;
            case 2://오렌지
                hp += 45;
                break;
            case 3://키위, 박쥐 구역
            case 4:
            case 6://레몬, 버섯 구역
            case 7://체리, 버섯 구역
            case 8://사과, 문지기 구역
            case 9://바나나, 문지기 구역
                hp += 50;
                break;
            case 5://당근, 나무 구역
            case 10://도토리, 보스구역
            case 11:
            case 12:
                hp += 60;
                break;
        }
        foodNum[f-1] = 0;
        if(hp >= hpMax)
        {
            hp = hpMax;
        }
        HP_Update();
    }

    public void SavePlayerData()//플레이어 진행 데이터 저장, 게임 종료 시 실행
    {
        SoundManager.instance.PlayClickSound();
        PlayerPrefs.SetInt("Level", level);//레벨 저장
        PlayerPrefs.SetInt("EXP", exp);
        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.SetInt("SP", sp);
        PlayerPrefs.SetInt("MainCount", GameDirector.instance.mainCount);

        PlayerPrefs.SetFloat("PosX", GameDirector.instance.Player.transform.position.x); //플레이어의 위치 정보 저장
        PlayerPrefs.SetFloat("PosY", GameDirector.instance.Player.transform.position.y);
        PlayerPrefs.SetFloat("PosZ", GameDirector.instance.Player.transform.position.z);
        PlayerPrefs.SetFloat("RotY", GameDirector.instance.Player.transform.eulerAngles.y);

        PlayerPrefs.SetInt("Slime1", aliveMonsters["Slime"]);//남은 몬스터 수 저장
        PlayerPrefs.SetInt("Slime2", aliveMonsters["Slime2"]);
        PlayerPrefs.SetInt("Turtle", aliveMonsters["Turtle"]);
        PlayerPrefs.SetInt("Log", aliveMonsters["Log"]);
        PlayerPrefs.SetInt("Bat", aliveMonsters["Bat"]);
        PlayerPrefs.SetInt("Mushroom", aliveMonsters["Mushroom"]);

        if(GameDirector.instance.mainCount != 10)//보스전에서 보스가 죽기 전에 게임 종료 시 보스를 물리치는 단계에서 먹은 음식은 저장X 
        {
            Save_Food_Info();
        }
        PlayerPrefs.Save(); //세이브

        Application.Quit();
    }

    public bool load = false; //로드 중 true
    public Text loadvalue;
    void LoadPlayerData()
    {
        load = true;
        GameDirector.instance.mainCount = PlayerPrefs.GetInt("MainCount");

        Vector3 PlayerPos = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));
        if (GameDirector.instance.mainCount >= 5 && GameDirector.instance.mainCount <= 11)
        {
            GameDirector.instance.friend_slime.transform.position = PlayerPos + new Vector3(2f, 0, -3f);//슬라임동료 위치 조정
            GameDirector.instance.friend_slime.transform.rotation = Quaternion.Euler(0, PlayerPrefs.GetFloat("RotY"), 0);
            GameDirector.instance.friend_slime.SetActive(true);
        }
        if (GameDirector.instance.mainCount >= 8 && GameDirector.instance.mainCount <= 11)
        {
            GameDirector.instance.friend_mushroom.transform.position = PlayerPos + new Vector3(-2f, 0, -3f);//버섯동료 위치 조정
            GameDirector.instance.friend_mushroom.transform.rotation = Quaternion.Euler(0, PlayerPrefs.GetFloat("RotY"), 0);
            GameDirector.instance.friend_mushroom.SetActive(true);
            Destroy(GameDirector.instance.wall);
        }

        Invoke(nameof(Black_Off), 2.2f);
        if(GameDirector.instance.mainCount == 0)//처음 시작이면
        {
            GameDirector.instance.Player.GetComponent<CharacterController>().enabled = false;//이동을 위해 잠시 캐릭터 컨트롤러 꺼두고
            GameDirector.instance.Player.transform.position = new Vector3(280f, 0, 80f);
            GameDirector.instance.Player.transform.rotation = Quaternion.Euler(0, 180f, 0);
            GameDirector.instance.Fox_Cant_Move();
            GameDirector.instance.Invoke(nameof(GameDirector.instance.MainProgress), 2.3f);
            aliveMonsters.Add("Slime", 3);
            aliveMonsters.Add("Slime2", 1);
            aliveMonsters.Add("Turtle", 3);
            aliveMonsters.Add("Log", 4);
            aliveMonsters.Add("Bat", 4);
            aliveMonsters.Add("Mushroom", 4);
        }
        else
        {
            GameDirector.instance.Player.GetComponent<CharacterController>().enabled = false;//이동을 위해 잠시 캐릭터 컨트롤러 꺼두고
            GameDirector.instance.Player.transform.position = PlayerPos; //저장된 위치로 이동시킴
            GameDirector.instance.Player.transform.rotation = Quaternion.Euler(0, PlayerPrefs.GetFloat("RotY"), 0);
            GameDirector.instance.Player.GetComponent<CharacterController>().enabled = true;

            level = PlayerPrefs.GetInt("Level");
            Level_Update();

            exp = PlayerPrefs.GetInt("EXP");
            hp = PlayerPrefs.GetInt("HP");
            sp = PlayerPrefs.GetInt("SP");
            EXP_Update();
            HP_Update();
            SP_Update();

            if (GameDirector.instance.mainCount >= 6 && GameDirector.instance.mainCount <= 8)//펜스 부수고 난 이후, 문지기 발견 전
            {
                Destroy(GameDirector.instance.fenceObject);//펜스 오브젝트 파괴
                SoundManager.instance.PlaySecondBgm();//배경음악 변경
            }
            else if(GameDirector.instance.mainCount <= 5)//펜스 부수기 전
            {
                SoundManager.instance.PlayFirstBgm();
            }
            else if(GameDirector.instance.mainCount == 9)//문지기 발견 후
            {
                Destroy(GameDirector.instance.fenceObject);//펜스 오브젝트 파괴
                SoundManager.instance.PlayThirdBgm();//배경음악 변경
                Destroy(GameDirector.instance.gkDiscover);//문지기 감지 오브젝트 파괴, 콜라이더가 있어서 통행에 방해될 수 있음

            }
            else if(GameDirector.instance.mainCount > 9 && GameDirector.instance.mainCount <= 10)//문지기 쓰러뜨린 후
            {
                Destroy(GameDirector.instance.fenceObject);//펜스 오브젝트 파괴
                MonsterList.instance.monsterList[6].list[0].tag = "Untagged";//태그 수정
                Destroy(MonsterHPBar.instance.dog);//문지기 파괴
                Destroy(GameDirector.instance.gkDiscover);
                GameDirector.instance.bossGate.SetTrigger("Open");
                SoundManager.instance.PlayBossBgm();//배경음악 변경
            }
            else if(GameDirector.instance.mainCount >= 13)//모든 게 끝난 뒤
            {
                Destroy(GameDirector.instance.fenceObject);//펜스 오브젝트 파괴
                MonsterList.instance.monsterList[6].list[0].tag = "Untagged";//태그 수정
                Destroy(MonsterHPBar.instance.dog);//문지기 파괴
                Destroy(GameDirector.instance.gkDiscover);
                GameDirector.instance.bossGate.SetTrigger("Open");
                Destroy(MonsterHPBar.instance.boss);//보스 파괴
                SoundManager.instance.PlayEndingBgm();//배경음악 변경
                GameDirector.instance.resetButton.SetActive(true);//리셋버튼 활성화
            }
            //GameDirector.instance.ThirdPersonCamera.SetActive(true);
            CameraController.instance.SetFixedState(false);
            aliveMonsters["Slime"] = PlayerPrefs.GetInt("Slime");
            aliveMonsters["Slime2"] = PlayerPrefs.GetInt("Slime2");
            aliveMonsters["Turtle"] = PlayerPrefs.GetInt("Turtle");
            aliveMonsters["Log"] = PlayerPrefs.GetInt("Log");
            aliveMonsters["Bat"] = PlayerPrefs.GetInt("Bat");
            aliveMonsters["Mushroom"] = PlayerPrefs.GetInt("Mushroom");
            Load_Remaining_Monsters();
            Load_Food();
        }

        load = false;
    }

    public void Load_Remaining_Monsters()//남은 몬스터 수만큼만 활성
    {
        for (int i = 3; i > aliveMonsters["Slime"]; i--)//슬라임1
        {
            MonsterList.instance.monsterList[0].list[i - 1].tag = "Untagged";//태그 수정
            MonsterList.instance.monsterList[0].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[0].list[i - 1]);
        }
        for (int i = 3; i > aliveMonsters["Turtle"]; i--)//거북이
        {
            MonsterList.instance.monsterList[2].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[2].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[2].list[i - 1]);
        }
        if(aliveMonsters["Slime2"] == 0)//슬라임2
        {
            MonsterList.instance.monsterList[1].list[0].tag = "Untagged";
            MonsterList.instance.monsterList[1].list[0].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[1].list[0]);
        }
        for (int i = 4; i > aliveMonsters["Log"]; i--)//나무
        {
            MonsterList.instance.monsterList[3].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[3].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[3].list[i - 1]);
        }
        for (int i = 4; i > aliveMonsters["Bat"]; i--)//박쥐
        {
            MonsterList.instance.monsterList[4].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[4].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[4].list[i - 1]);
        }
        for (int i = 4; i > aliveMonsters["Mushroom"]; i--)//버섯
        {
            MonsterList.instance.monsterList[5].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[5].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[5].list[i - 1]);
        }
    }

    //음식 정보 배열, 1이면 아직 안 먹은 것 0이면 먹은 것
    public int[] foodNum = new int[12] {1,1,1,1,1,1,1,1,1,1,1,1}; // 순서 : 0토마토 1오렌지 2키위 3키위1 4당근 5레몬 6체리 7사과 8바나나 9 10 11도토리
    public void Save_Food_Info()//음식 정보 저장
    {
        string strArr = ""; // 문자열 생성

        for (int i = 0; i < foodNum.Length; i++) // 배열과 ','를 번갈아가며 저장
        {
            strArr = strArr + foodNum[i];
            if (i < foodNum.Length - 1) // 최대 길이의 -1까지만 ,를 저장
            {
                strArr = strArr + ",";
            }
        }

        PlayerPrefs.SetString("Foods", strArr); // PlyerPrefs에 문자열 형태로 저장
        PlayerPrefs.Save(); //세이브
    }

    public GameObject[] foods; //음식 오브젝트 배열
    void Load_Food()
    {
        string[] dataArr = PlayerPrefs.GetString("Foods").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

        for (int i = 0; i < dataArr.Length; i++)
        {
            foodNum[i] = System.Convert.ToInt32(dataArr[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장       
            if(foodNum[i] == 0)//이미 먹은 음식이면
            {
                Destroy(foods[i]);//오브젝트 파괴
            }
        }
    }

    void Black_Off()
    {
        BlackScreen.SetActive(false);
    }

    void Check_All_Died()//보스, 문지기 제외 몬스터 다 죽었는지 확인 후 두번째 동료 등장
    {
        if (aliveMonsters["Log"] == 0 && aliveMonsters["Bat"] == 0 && aliveMonsters["Mushroom"] == 0 && GameDirector.instance.mainCount == 7)
        {
            BlackScreen.SetActive(true);
            blackAnimator.SetTrigger("BlackOn");
            GameDirector.instance.Fox_Cant_Move();
            GameDirector.instance.Invoke(nameof(GameDirector.instance.Ready_To_Talk), 2.2f);
            DialogueController.instance.SetDialogue(10);

        }
    }

    Dictionary<string, int> aliveMonsters = new Dictionary<string, int>();

    public void UpdateMonsterCount(string monsterName, int changeValue)
    {
        aliveMonsters[monsterName] += changeValue;
    }



}
