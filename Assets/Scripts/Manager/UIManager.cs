using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Health UI")]
    [SerializeField] private Text currnetHealthText;
    [SerializeField] private Text maxHealthText; //생명력 최대치 텍스트
    [SerializeField] private Image healthBar;

    [Header("Stamina UI")]
    [SerializeField] private Text currnetStaminaText; //스태미나 텍스트
    [SerializeField] private Text maxStaminaText; //스태미나 최대치 텍스트
    [SerializeField] private Image staminaBar;//스태미나 

    [Header("Level UI")]
    [SerializeField] private Text currentLevelText;//레벨 텍스트
    [SerializeField] private Animator levelBoardAnimator;//레벨업 보드 애니메이터
    [SerializeField] private Text levelText; //레벨업 보드의 레벨 텍스트
    [SerializeField] private Text levelGuideMessage; //현재 레벨에서 무엇을 새로이 할 수 있는지 안내해주는 메세지

    [Header("Exp UI")]
    [SerializeField] private Text currentExpText;
    [SerializeField] private Text maxExpText; //경험치 최대치 텍스트
    [SerializeField] private Image expBar;
    [SerializeField] private Text middleSlash;//슬래시 텍스트
    [SerializeField] private Text maxText;//5렙일 때 경험치란에 max 표시

    [Header("Other UI")]
    [SerializeField] private GameObject damageText = null;//플레이어 데미지 프리팹
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject blackScreen;//플레이어 죽음 시 블랙스크린

    public Animator BlackAnimator { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        BlackAnimator = blackScreen.GetComponent<Animator>();
    }

    public void StartBlackOut()//플레이어 죽음 시 검은 화면 전환
    {
        ActiveBlackScreen();
        DialogueManager.instance.SetDialogue(1);
        GameDirector.instance.Invoke(nameof(GameDirector.instance.Start_Talk), 3.5f);
    }

    public void EndBlackOut() // 부활, 화면 다시 밝아지고 플레이어 처음 위치로 이동, 능력치 변화
    {
        CameraController.instance.SetFieldOfView(35f);
        CameraController.instance.SetYAxisValue(0.6f);
        CameraController.instance.SetXAxisValue(0f);

        Player.instance.PlayerPos = new Vector3(280, 0, 80);//플레이어 위치 처음으로
        Player.instance.PlayerRot = Quaternion.Euler(0, 180, 0);//플레이어 회전, 앞을 보도록
        if (GameDirector.instance.mainCount >= 5)
        {
            GameDirector.instance.friend_slime.transform.position = Player.instance.PlayerPos + new Vector3(2f, 0, -3f);//슬라임동료 위치 조정
        }
        if (GameDirector.instance.mainCount >= 8)
        {
            GameDirector.instance.friend_mushroom.transform.position = Player.instance.PlayerPos + new Vector3(-2f, 0, -3f);//슬라임동료 위치 조정
        }

        Player.instance.StatusComponent.CurrentHealth = 10;
        UpdatePlayerHealthUI();

        MyTaskManager.instance.ExecuteAfterDelay(() => BlackAnimator.SetTrigger("BlackOff"), 0.8f);
        MyTaskManager.instance.ExecuteAfterDelay(Player.instance.Revive, 2.5f);
    }

    public void ActiveBlackScreen()
    {
        blackScreen.SetActive(true);
        BlackAnimator.SetTrigger("BlackOn");
    }

    public void DeactiveBlackScreen()
    {
        blackScreen.SetActive(false);
    }

    public void CloseLevelUpBoard()
    {
        SoundManager.instance.PlayClickSound();
        levelBoardAnimator.SetTrigger("Up");

        int level = Player.instance.StatusComponent.CurrentLevel;
        if (level == 3)//3렙, 새로운 기술 안내
        {
            DialogueManager.instance.SetDialogue(9);
            GameDirector.instance.Start_Talk();
        }
        if (level == 5)//5렙, 새로운 기술 안내
        {
            DialogueManager.instance.SetDialogue(12);
            GameDirector.instance.Start_Talk();
        }
    }

    public void UpdatePlayerHealthUI()//생명력 갱신, 레벨에 따라 생명력바 게이지 차는 양 달라짐
    {
        float health = Player.instance.StatusComponent.CurrentHealth;
        healthBar.fillAmount = health / Player.instance.StatusComponent.MaxHealth;
        currnetHealthText.text = health.ToString(); //화면상의 생명력 텍스트 변경
    }

    public void UpdatePlayerStaminaUI()
    {
        float stamina = Player.instance.StatusComponent.CurrentStamina;
        staminaBar.fillAmount = stamina / Player.instance.StatusComponent.MaxStamina;
        currnetStaminaText.text = stamina.ToString();
    }

    public void UpdatePlayerExpUI()
    {
        float exp = Player.instance.StatusComponent.CurrentExp;
        currentExpText.text = exp.ToString();
        expBar.fillAmount = exp / Player.instance.StatusComponent.MaxExp;
    }

    public void UpdatePlayerLevelUI()
    {
        int level = Player.instance.StatusComponent.CurrentLevel;
        currentLevelText.text = level.ToString();
        if (level == 5)
        {
            middleSlash.gameObject.SetActive(false);//지금까지 표시된 경험치 텍스트들 모두 비활성화
            currentExpText.gameObject.SetActive(false);
            maxExpText.gameObject.SetActive(false);
            maxText.gameObject.SetActive(true);//max 텍스트 활성화
            expBar.fillAmount = 1f;
        }
        else
        {
            maxExpText.text = Player.instance.StatusComponent.MaxExp.ToString();
        }

        maxHealthText.text = Player.instance.StatusComponent.MaxHealth.ToString();
        maxStaminaText.text = Player.instance.StatusComponent.MaxStamina.ToString();

        if ((PlayerPrefs.GetInt("Level") == 0 && level == 1) || PlayerPrefs.GetInt("Level") == level) return; // 레벨업 상태가 아님

        switch (level)
        {
            case 2:
                levelGuideMessage.text = "이제 울타리를 부술 수 있습니다.";
                break;
            case 4:
                levelGuideMessage.text = "";
                break;
            case 3:
            case 5:
                levelGuideMessage.text = "새로운 공격 기술을 얻었습니다!";
                break;
        }

        levelText.text = level.ToString();
        levelBoardAnimator.SetTrigger("Down");
    }

    public void ShowDamageText(GameObject target, float damage)
    {
        GameObject dText = Instantiate(damageText, target.transform.position, Quaternion.identity, canvas.transform);
        dText.GetComponent<DamageText>().SetDamage(damage);

        if (target.name.Contains("Fox"))  // 데미지 입은 대상에 따라 색상 변경
            dText.GetComponent<Text>().color = new Color(255 / 255f, 105 / 255f, 0 / 255f);
        else
            dText.GetComponent<Text>().color = new Color(200 / 255f, 0 / 255f, 30 / 255f);

        float randomX = Random.Range(-2f, 2f);
        dText.transform.position = Camera.main.WorldToScreenPoint(dText.transform.position + new Vector3(randomX, 1f, 0));
    }
}
