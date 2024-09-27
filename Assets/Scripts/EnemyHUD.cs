using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour //Canvas에 추가되어있음
{
    private readonly Color RED_COLOR = new Color(255 / 255f, 50 / 255f, 50 / 255f, 1f);
    private readonly Color WHITE_COLOR = new Color(255 / 255f, 255 / 255f, 255 / 255f, 1f);


    [SerializeField] GameObject hpPrefeb = null; //hp바 프리팹
    [SerializeField] GameObject levelPrefeb = null;//몬스터 레벨 프리팹
    [SerializeField] GameObject arrowPrefeb = null; //화살표 프리팹
    Dictionary<Transform, GameObject> hpBarDict = new Dictionary<Transform, GameObject>();
    Dictionary<Transform, GameObject> levelDict = new Dictionary<Transform, GameObject>();


    Player player;
    Camera cam = null;
    GameObject arrow;

    Transform targetTransform;

    public bool IsTargetting {get; private set;}

    public static EnemyHUD instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        player = Player.instance;
        cam = Camera.main;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //살아있는 몬스터 전부 저장
        for (int i = 0; i < enemies.Length; i++)
        {
            Transform enemyTransform = enemies[i].transform;

            GameObject hpBar = Instantiate(hpPrefeb, enemyTransform.position, Quaternion.identity, transform);
            hpBarDict[enemyTransform] = hpBar;

            GameObject level = Instantiate(levelPrefeb, enemyTransform.position, Quaternion.identity, transform);
            levelDict[enemyTransform] = level;
        }

        if (!(player.StatusComponent.CurrentLevel == 2 && GameDirector.instance.mainCount == 4))//아직 울타리 부수기 전(슬라임 등장 전)이면
        {
            arrow = Instantiate(arrowPrefeb, new Vector3(), Quaternion.identity, transform); //몬스터 위치에 화살표 프리팹 생성
            arrow.SetActive(false);
        }
    }

    void Update()
    {
        targetTransform = player.GetTarget()?.transform;

        if(targetTransform != null)//몬스터가 타겟으로 지정됐을 때
        {
            UpdateTargetHUD();
        }

        if (targetTransform != null && targetTransform.GetComponent<IEnemyController>().IsDead())//타겟 죽었을 경우
        {
           RemoveTargetHUD();
        }

        if(player.StatusComponent.CurrentLevel == 2 && GameDirector.instance.mainCount == 4)//2렙, 울타리 부수기 전
        {
            if(arrow == null)
            {
                arrow = Instantiate(arrowPrefeb,GameDirector.instance.right_Fence.transform.position, Quaternion.identity, transform);
            }

            if(arrow.transform.position.z < 0) arrow.SetActive(false);
            else arrow.SetActive(true);
            
            Vector3 fencePos = GameDirector.instance.right_Fence.transform.position;
            arrow.transform.position = cam.WorldToScreenPoint(fencePos + new Vector3(-1.5f, 2.5f, -1.5f));
        }
    }

   private void UpdateHUDPosition(Transform enemyTransform, float arrowOffsetY, float hpBarOffsetY, float levelOffsetY)
    {
        Vector3 position = enemyTransform.position;
        arrow.transform.position = cam.WorldToScreenPoint(position + new Vector3(0, arrowOffsetY, 0));
        hpBarDict[enemyTransform].transform.position = cam.WorldToScreenPoint(position + new Vector3(0, hpBarOffsetY, 0));
        levelDict[enemyTransform].transform.position = cam.WorldToScreenPoint(position + new Vector3(0, levelOffsetY, 0));
    }

    void UpdateTargetHUD()
    {
        if (hpBarDict.TryGetValue(targetTransform, out GameObject hpBar) && levelDict.TryGetValue(targetTransform, out GameObject level))
        {
            hpBar.GetComponent<Image>().color = RED_COLOR;
            level.GetComponent<Image>().color = WHITE_COLOR;

            int levelValue = targetTransform.GetComponent<IEnemyController>().GetLevel();
            level.transform.GetChild(0).GetComponent<Text>().text = levelValue.ToString();
            level.transform.GetChild(0).GetComponent<Text>().color = WHITE_COLOR;
            arrow.transform.position = hpBar.transform.position;
            arrow.SetActive(true);
        }

        if (targetTransform.name.Contains("Dog"))//문지기
        {
            UpdateHUDPosition(targetTransform, 4f, 3.2f, 3.5f);
        }
        else if(targetTransform.name.Contains("Dragon"))//보스
        {
            UpdateHUDPosition(targetTransform, 5.2f, 4f, 4.5f);
        }
        else
        {
            UpdateHUDPosition(targetTransform, 2.7f, 1.5f, 1.8f);
        }
    }

    void RemoveTargetHUD()
    {
        Destroy(hpBarDict[targetTransform]);
        hpBarDict.Remove(targetTransform);
        Destroy(levelDict[targetTransform]);
        levelDict.Remove(targetTransform);

        player.SetTarget(null);
        arrow.SetActive(false);
    }

    public void DecreaseHealthUI(Transform enemy, float dam)//공격받았을 때 체력바 감소
    {
        if(GameDirector.instance.mainCount == 10 && targetTransform == null)//타겟 없지만 보스전일 때
        {
            hpBarDict[enemy].GetComponent<Image>().fillAmount -= dam / GameObject.Find("Monster_Dragon_Boss").GetComponent<IEnemyController>().GetMaxHealth();
        }
        else
        {
            hpBarDict[enemy].GetComponent<Image>().fillAmount -= dam / enemy.GetComponent<IEnemyController>().GetMaxHealth();//몬스터의 본래 체력에 따라 체력바 각각 다르게 감소  
        }           
    }

    public void ResetHP()//보스나 문지기 체력 회복 함수
    {
        if(GameDirector.instance.mainCount == 9)//문지기와 전투
        {
            if (hpBarDict.TryGetValue(GameObject.Find("Monster_DogKnight")?.transform, out GameObject hpBar))
            {
                hpBar.GetComponent<Image>().fillAmount = 1f;
            }
        }
        else if(GameDirector.instance.mainCount > 9)//보스와 전투
        {
            if (hpBarDict.TryGetValue(GameObject.Find("Monster_Dragon_Boss")?.transform, out GameObject hpBar))
            {
                hpBar.GetComponent<Image>().fillAmount = 1f;
            }
        }
    }

    public void DisappearMonsterInfo()//기존 타겟 정보 안 보이게
    {
        arrow.SetActive(false);
        
        if(targetTransform == null) return;

        Color transparentColor = WHITE_COLOR;
        transparentColor.a = 0f;

        levelDict[targetTransform].GetComponent<Image>().color = transparentColor;//몬스터 레벨 안 보이게 만듬
        levelDict[targetTransform].transform.GetChild(0).GetComponent<Text>().color = transparentColor;
        hpBarDict[targetTransform].GetComponent<Image>().color = transparentColor; //hp바를 안 보이게 만듬
    }

    public void DeactiveArrow()
    {
        arrow.SetActive(false);
    }
}
