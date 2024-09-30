using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public Dictionary<string, int> AliveMonsters { get; private set; }

    //음식 정보 배열, 1이면 아직 안 먹은 것 0이면 먹은 것
    private int[] foodNum = new int[13] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; // 순서 : 0토마토 1오렌지 2,3키위 4,5당근 6레몬 7체리 8사과 9바나나 10,11,12도토리
    [SerializeField] private GameObject[] foods; //음식 오브젝트 배열

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        AliveMonsters = new Dictionary<string, int>();
        LoadPlayerData();
    }

    public void ResetData()//데이터 초기화
    {
        PlayerPrefs.DeleteAll();//저장된 것 전부 삭제
        Application.Quit();//게임 종료
    }

    public void SavePlayerData()//플레이어 진행 데이터 저장, 게임 종료 시 실행
    {
        SoundManager.instance.PlayClickSound();

        Player.instance.StatusComponent.SavePlayerStatus();

        PlayerPrefs.SetInt("MainCount", GameManager.instance.MainCount);

        PlayerPrefs.SetFloat("PosX", Player.instance.PlayerPos.x); //플레이어의 위치 정보 저장
        PlayerPrefs.SetFloat("PosY", Player.instance.PlayerPos.y);
        PlayerPrefs.SetFloat("PosZ", Player.instance.PlayerPos.z);
        PlayerPrefs.SetFloat("RotY", Player.instance.PlayerRot.eulerAngles.y);

        PlayerPrefs.SetInt("Slime", AliveMonsters["Slime"]);//남은 몬스터 수 저장
        PlayerPrefs.SetInt("Slime2", AliveMonsters["Slime2"]);
        PlayerPrefs.SetInt("Turtle", AliveMonsters["Turtle"]);
        PlayerPrefs.SetInt("Log", AliveMonsters["Log"]);
        PlayerPrefs.SetInt("Bat", AliveMonsters["Bat"]);
        PlayerPrefs.SetInt("Mushroom", AliveMonsters["Mushroom"]);

        if (GameManager.instance.MainCount != 10)//보스전에서 보스가 죽기 전에 게임 종료 시 보스를 물리치는 단계에서 먹은 음식은 저장X 
        {
            SaveFood();
        }
        PlayerPrefs.Save(); //세이브
        Application.Quit();
    }

    private void LoadPlayerData()
    {
        GameManager.instance.MainCount = PlayerPrefs.GetInt("MainCount");

        if (GameManager.instance.MainCount == 0)//처음 시작이면
        {
            Player.instance.GetComponent<CharacterController>().enabled = false;//이동을 위해 잠시 캐릭터 컨트롤러 꺼두고
            Player.instance.PlayerPos = new Vector3(280f, 0, 80f);
            Player.instance.PlayerRot = Quaternion.Euler(0, 180f, 0);

            Player.instance.DisableMovement();
            GameManager.instance.Invoke(nameof(GameManager.instance.MainProgress), 2.3f);
            AliveMonsters.Add("Slime", 3);
            AliveMonsters.Add("Slime2", 1);
            AliveMonsters.Add("Turtle", 3);
            AliveMonsters.Add("Log", 4);
            AliveMonsters.Add("Bat", 4);
            AliveMonsters.Add("Mushroom", 4);
        }
        else
        {
            Player.instance.GetComponent<CharacterController>().enabled = false;//이동을 위해 잠시 캐릭터 컨트롤러 꺼두고

            Vector3 playerPos = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));
            Player.instance.PlayerPos = playerPos; //저장된 위치로 이동시킴
            Player.instance.PlayerRot = Quaternion.Euler(0, PlayerPrefs.GetFloat("RotY"), 0);
            Player.instance.GetComponent<CharacterController>().enabled = true;

            Player.instance.StatusComponent.LoadPlayerStatus();

            GameManager.instance.UpdateWorldState();

            CameraController.instance.SetFixedState(false);
            AliveMonsters["Slime"] = PlayerPrefs.GetInt("Slime");
            AliveMonsters["Slime2"] = PlayerPrefs.GetInt("Slime2");
            AliveMonsters["Turtle"] = PlayerPrefs.GetInt("Turtle");
            AliveMonsters["Log"] = PlayerPrefs.GetInt("Log");
            AliveMonsters["Bat"] = PlayerPrefs.GetInt("Bat");
            AliveMonsters["Mushroom"] = PlayerPrefs.GetInt("Mushroom");
            LoadAliveMonsters();
            LoadFood();

        }

        UIManager.instance.UpdatePlayerLevelUI();
        UIManager.instance.UpdatePlayerExpUI();
        UIManager.instance.UpdatePlayerHealthUI();
        UIManager.instance.UpdatePlayerStaminaUI();

        UIManager.instance.Invoke(nameof(UIManager.instance.DeactiveBlackScreen), 2.2f);
    }

    public void UpdateMonsterCount(string monsterName, int changeValue)
    {
        AliveMonsters[monsterName] += changeValue;
    }


    private void LoadAliveMonsters()//남은 몬스터 수만큼만 활성
    {
        for (int i = 3; i > AliveMonsters["Slime"]; i--)//슬라임1
        {
            MonsterList.instance.monsterList[0].list[i - 1].tag = "Untagged";//태그 수정
            MonsterList.instance.monsterList[0].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[0].list[i - 1]);
        }
        for (int i = 3; i > AliveMonsters["Turtle"]; i--)//거북이
        {
            MonsterList.instance.monsterList[2].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[2].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[2].list[i - 1]);
        }
        if (AliveMonsters["Slime2"] == 0)//슬라임2
        {
            MonsterList.instance.monsterList[1].list[0].tag = "Untagged";
            MonsterList.instance.monsterList[1].list[0].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[1].list[0]);
        }
        for (int i = 4; i > AliveMonsters["Log"]; i--)//나무
        {
            MonsterList.instance.monsterList[3].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[3].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[3].list[i - 1]);
        }
        for (int i = 4; i > AliveMonsters["Bat"]; i--)//박쥐
        {
            MonsterList.instance.monsterList[4].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[4].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[4].list[i - 1]);
        }
        for (int i = 4; i > AliveMonsters["Mushroom"]; i--)//버섯
        {
            MonsterList.instance.monsterList[5].list[i - 1].tag = "Untagged";
            MonsterList.instance.monsterList[5].list[i - 1].GetComponent<IEnemyController>().Disable();
            Destroy(MonsterList.instance.monsterList[5].list[i - 1]);
        }
    }

    public void EatFood(int fNum)
    {
        SoundManager.instance.PlayClickSound();

        foodNum[fNum - 1] = 0;

        int hp = 0;
        switch (fNum)
        {
            case 1://토마토
                hp = 40;
                break;
            case 2://오렌지
                hp = 45;
                break;
            case 3://키위, 박쥐 구역
            case 4://키위, 박쥐 구역
            case 7://레몬, 버섯 구역
            case 8://체리, 버섯 구역
            case 9://사과, 문지기 구역
            case 10://바나나, 문지기 구역
                hp = 50;
                break;
            case 5://당근, 나무 구역
            case 6://당근, 나무 구역
            case 11://도토리, 보스구역
            case 12://도토리, 보스구역
            case 13://도토리, 보스구역
                hp = 60;
                break;
        }
        Player.instance.StatusComponent.ModifyHealth(hp);
    }


    private void SaveFood()//음식 정보 저장
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

    private void LoadFood()
    {
        string[] dataArr = PlayerPrefs.GetString("Foods").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

        for (int i = 0; i < dataArr.Length; i++)
        {
            foodNum[i] = System.Convert.ToInt32(dataArr[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장       
            if (foodNum[i] == 0)//이미 먹은 음식이면
            {
                Destroy(foods[i]);//오브젝트 파괴
            }
        }
    }


}
