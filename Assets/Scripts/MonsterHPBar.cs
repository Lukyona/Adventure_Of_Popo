using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    [SerializeField] GameObject hpPrefeb = null; //hp바 프리팹
    [SerializeField] GameObject arrowPrefeb = null; //화살표 프리팹
    List<Transform> transformtList = new List<Transform>(); //몬스터들 위치 리스트 
    public List<GameObject> hpBarList = new List<GameObject>();//몬스터 hp바 리스트
    [SerializeField] GameObject monLevelPrefeb = null;//몬스터 레벨 프리팹
    public List<GameObject> levelList = new List<GameObject>();//몬스터 레벨 리스트
    [SerializeField] GameObject damageText = null;//몬스터 데미지 프리팹

    Camera cam = null;

    Color color = new Color(255/255f, 50/255f, 50/255f);//빨간색 컬러
    Color color1 = new Color(255/255f, 255/255f, 255/255f);//하얀색 컬러

    public static MonsterHPBar instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        cam = Camera.main;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Monster"); //살아있는 몬스터 전부 저장
        for (int i = 0; i < objects.Length; i++)
        {
            transformtList.Add(objects[i].transform);
            GameObject hpBar = Instantiate(hpPrefeb, objects[i].transform.position, Quaternion.identity, transform); //몬스터 위치에 hp바 프리팹 생성
            hpBarList.Add(hpBar);//생성된 HP바를 리스트에 추가
            GameObject mon_level = Instantiate(monLevelPrefeb, objects[i].transform.position, Quaternion.identity, transform);//레벨 정보 프리팹 생성
            levelList.Add(mon_level);//생성된 몬스터 레벨을 리스트에 추가
        }
        if (PlayerInfoManager.instance.level == 2 && GameDirector.instance.mainCount == 4)//아직 울타리 부수기 전(슬라임 등장 전)이면
        {
            Fence_arrow();//울타리 표시
        }
    }

    public bool targetIn = false;//타겟이 있으면 true
    public int num;//타겟 인덱스
    GameObject arrow;
    void Update()
    {
        if(PlayerAttack.instance.target != null && !targetIn)//몬스터가 타겟으로 지정됐을 때
        {
            targetIn = true;
            for (int i = 0; i < transformtList.Count; i++)
            {
                if(PlayerAttack.instance.target.transform == transformtList[i].transform)//타겟인 몬스터 찾고 일치하면
                {
                    color.a = 255f;
                    color1.a = 255f;
                    hpBarList[i].GetComponent<Image>().color = color; //hp바를 보이게 만듬
                    levelList[i].GetComponent<Image>().color = color1;//레벨 정보
                    if (PlayerAttack.instance.target.name.Contains("Slime") || PlayerAttack.instance.target.name.Contains("Turtle"))
                    {
                        levelList[i].transform.GetChild(0).GetComponent<Text>().text = 1.ToString();
                    }
                    else if (PlayerAttack.instance.target.name.Contains("Log"))
                    {
                        levelList[i].transform.GetChild(0).GetComponent<Text>().text = 2.ToString();
                    }
                    else if (PlayerAttack.instance.target.name.Contains("Bat"))
                    {
                        levelList[i].transform.GetChild(0).GetComponent<Text>().text = 3.ToString();
                    }
                    else if (PlayerAttack.instance.target.name.Contains("Mushroom") || PlayerAttack.instance.target.name.Contains("Dog"))
                    {
                        levelList[i].transform.GetChild(0).GetComponent<Text>().text = 4.ToString();
                    }
                    else if (PlayerAttack.instance.target.name.Contains("Dragon"))
                    {
                        levelList[i].transform.GetChild(0).GetComponent<Text>().text = 5.ToString();
                    }
                    levelList[i].transform.GetChild(0).GetComponent<Text>().color = color1;//레벨 텍스트
                    arrow = Instantiate(arrowPrefeb, hpBarList[i].transform.position, Quaternion.identity, transform); //몬스터 위치에 화살표 프리팹 생성
                    arrow.SetActive(true);
                    num = i;
                }
            }
        }
        if(PlayerAttack.instance.target == null && targetIn)
        {
            DisappearMonsterInfo();
        }

        if (PlayerAttack.instance.target != null)//타겟 있을 때만 화살표,hp바,레벨 정보가 몬스터 따라다니게
        {      
            if (PlayerAttack.instance.target.name.Contains("Dog"))//문지기
            {
                arrow.transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 4f, 0));
                hpBarList[num].transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 3.2f, 0));//HP바가 몬스터 위치 따라 이동
                levelList[num].transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 3.5f, 0));//레벨 정보가 몬스터 위치 따라 이동
            }
            else if(PlayerAttack.instance.target.name.Contains("Dragon"))//보스
            {
                arrow.transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 5.2f, 0));
                hpBarList[num].transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 4f, 0));//HP바가 몬스터 위치 따라 이동
                levelList[num].transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 4.5f, 0));//레벨 정보가 몬스터 위치 따라 이동
            }
            else
            {
                for (int i = 0; i < transformtList.Count; i++)
                {
                    hpBarList[i].transform.position = cam.WorldToScreenPoint(transformtList[i].position + new Vector3(0, 1.5f, 0));//HP바가 몬스터 위치 따라 이동
                    levelList[i].transform.position = cam.WorldToScreenPoint(transformtList[i].position + new Vector3(0, 1.8f, 0));//레벨 정보가 몬스터 위치 따라 이동
                }
                arrow.transform.position = cam.WorldToScreenPoint(transformtList[num].position + new Vector3(0, 2.7f, 0));
            }
        }

        if(PlayerInfoManager.instance.level == 2 && GameDirector.instance.mainCount == 4)//2렙에 울타리 부수기 전
        {
            arrow.transform.position = cam.WorldToScreenPoint(GameDirector.instance.right_Fence.transform.position + new Vector3(-1.5f, 2.5f, -1.5f));
        }
    }

    public void Get_Damage(int dam)//공격받았을 때 체력바 감소
    {
        if(GameDirector.instance.mainCount == 10 && PlayerAttack.instance.target == null)//타겟 없지만 보스전일 때
        {
            hpBarList[0].GetComponent<Image>().fillAmount -= dam / MonsterHPBar.instance.boss.GetComponent<MonsterController>().health_info;
        }
        else//보스전 제외 모두
        {
            if (PlayerAttack.instance.target.name.Contains("Turtle"))//거북이만 유일하게 입는 데미지 다름
            {
                hpBarList[num].GetComponent<Image>().fillAmount -= 0.133f;//12데미지
            }
            else
            {
                if (GameDirector.instance.mainCount < 9)//보스/문지기 제외
                {
                    hpBarList[num].GetComponent<Image>().fillAmount -= dam / PlayerAttack.instance.target.GetComponent<EnemyAi>().health_info;//몬스터의 본래 체력에 따라 체력바 각각 다르게 감소  
                }
                else
                {
                    hpBarList[num].GetComponent<Image>().fillAmount -= dam / PlayerAttack.instance.target.GetComponent<MonsterController>().health_info;
                }
            }
        }           
    }

    public GameObject boss;//보스 오브젝트
    public GameObject dog;//문지기 오브젝트

    public void Recover_HP(float hp)//보스나 문지기 체력 회복 함수
    {
        if(GameDirector.instance.mainCount == 9)//문지기와 전투
        {
            hpBarList[1].GetComponent<Image>().fillAmount += hp / dog.GetComponent<MonsterController>().health_info;
        }
        else if(GameDirector.instance.mainCount > 9)//보스와 전투
        {
            hpBarList[0].GetComponent<Image>().fillAmount += hp / boss.GetComponent<MonsterController>().health_info;
        }
    }

    public void DisappearMonsterInfo()//기존 타겟 정보 안 보이게
    {
        targetIn = false;
        Destroy(arrow);
        color.a = 0f;
        levelList[num].GetComponent<Image>().color = color;//몬스터 레벨 안 보이게 만듬
        levelList[num].transform.GetChild(0).GetComponent<Text>().color = color;
        hpBarList[num].GetComponent<Image>().color = color; //hp바를 안 보이게 만듬
        if (PlayerAttack.instance.target != null)//타겟 죽었을 경우
        {
            if((GameDirector.instance.mainCount < 9 && PlayerAttack.instance.target.GetComponent<EnemyAi>().health <= 0)
                || (GameDirector.instance.mainCount >= 9 && PlayerAttack.instance.target.GetComponent<MonsterController>().health <= 0))
            {
                transformtList.Remove(transformtList[num]);
                Destroy(hpBarList[num]);
                hpBarList.Remove(hpBarList[num]);//hp바 리스트에서 소멸
                Destroy(levelList[num]);
                levelList.Remove(levelList[num]);//hp바 리스트에서 소멸
                PlayerAttack.instance.target = null;
            }          
        }
    }

    public void ShowDamage(int damage)
    {
        GameObject dText = Instantiate(damageText, hpBarList[num].transform.position, Quaternion.identity, transform);
        dText.GetComponent<DamageText>().damage = damage;
        dText.transform.position = cam.WorldToScreenPoint(transformtList[num].transform.position + new Vector3(2f, 1f, 0));
    }

    public void Fence_arrow()//올바른(부술 수 있는) 울타리 표시해주기
    {
        if(GameDirector.instance.mainCount == 5)//슬라임 등장 후 화살표 제거
        {
            Destroy(arrow);
        }
        else
        {
            arrow = Instantiate(arrowPrefeb,GameDirector.instance.right_Fence.transform.position, Quaternion.identity, transform); //펜스 위치에 화살표 프리팹 생성    
            arrow.SetActive(true);
        }
    }
}
