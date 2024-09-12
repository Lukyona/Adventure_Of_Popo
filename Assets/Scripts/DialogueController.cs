using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    [SerializeField] TextMeshProUGUI DialogueText;
    [SerializeField] Image NameTag;//캐릭터 이름칸, 캐릭터에 따라 색상 달라질 것
    [SerializeField] Text Name;//캐릭터의 이름

    int dialogueNum = 0;
    int index = 0;
    float speed = 0.02f;

    string[] sentences = null;
    bool isLineComplete = true; // 대사 출력 완료 상태면 true
    bool wait = false; //대사 넘어가기 방지
    bool goMainProgress = true; //메인 진행함수로 넘어가야할 때는 true

    Color slimeColor = new Color(255 / 255f, 50 / 255f, 80 / 255f);
    Color mushroomColor = new Color(40 / 255f, 200 / 255f, 0 / 255f);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && isLineComplete && GameDirector.instance.talking && !wait)
        {//대화 시작한 상태에서 스페이스키 혹은 마우스 왼쪽 클릭을 했을 때
            ShowDialogue();       
        }
    }

    public void ShowDialogue()
    {
        isLineComplete = false;
        if (index <= sentences.Length - 1) //인덱스는 0부터 시작, 대사 배열 길이보다 작으면 대사 출력 코루틴 시작
        {
            StartCoroutine(PrintDialogue());
        }
        else
        {
            GameDirector.instance.End_Talk();//대화 종료
            if (Player.instance.IsDead())//플레이어 죽음 상태면
            {
                UIManager.instance.Invoke(nameof(UIManager.instance.EndBlackOut),2f);//화면 밝아지기
            }
            else
            {
                if(goMainProgress)//메인 진행 함수로 가야할 때는
                {
                    goMainProgress = false;
                    GameDirector.instance.MainProgress();
                }               
            }
        }
    }

    IEnumerator PrintDialogue()
    {
        DialogueText.text = "";

        if(Player.instance.StatusComponent.CurrentLevel >= 2)//레벨이 2이상이면
        {
            CheckDialogueEvent();
        }

        foreach (char Character in sentences[index].ToCharArray())
        {
            DialogueText.text += Character; //문자 추가
            yield return new WaitForSeconds(speed); // 지정한 속도로 나오기
        }
        
        index++;//대사 하나 끝나면 인덱스 증가
        isLineComplete = true;
    }

    public void SetDialogue(int dNum)
    {
        dialogueNum = dNum;
        index = 0;//인덱스는 대화시작 전 0으로 초기화

        switch(dialogueNum)
        {
            case 0://주인공 소개
                sentences = new string[] {
                    "내 이름은 포포!",
                    "그 어떤 병이라도 낫게 해준다는 '엘릭서'를\n찾아 여기까지 왔어.",
                    "그 엘릭서라는 걸 마시면 우리 엄마도\n다시 건강해질 거야!",
                    "음.. 일단 주변부터 둘러볼까?",
                };
                break;
            case 1://플레이어 죽음
                sentences = new string[] {
                    "으으... 너무 아픈걸....",
                };
                goMainProgress = false;
                break;
            case 2://플레이어 울타리 부수기 가능
                NameTag.gameObject.SetActive(false);//이름표 없애고
                sentences = new string[] {
                    "이 울타리는 부술 수 있을 것 같다!",
                    "..잠깐!",
                    "응? 앗, 몬스터!",
                    "몬스터라니! 틀린 말은 아니지만..\n난 너가 싸우던 애들과는 달라.",//3
                    "에엥? 똑같이 생겼는데..",
                    "너.. 이 울타리 너머로 갈 생각이지?",
                    "응! 그런데 왜?",
                    "나도 데려가라. 내가 강한 건 아니지만..\n그래도 하나보단 둘이 낫지 않겠어?",//7
                    "그래, 좋아!",
                    "이렇게 쉽게 수락할 줄이야..;",
                    "아무튼 간에.. 내 이름은 '라임'이다.\n잘 부탁한다.",
                    "난 포포라고 해!",//11
                    "그래 꼬맹이, 그럼 이제 이 울타리 좀\n어떻게 해봐.",
                    "울타리가 부서질 때까지 숫자키 1번을\n눌러보자!",
                };
                break;
            case 3://플레이어 울타리 부수기 불가
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "레벨을 좀 더 올려야할 것 같다..",
                };
                goMainProgress = false;
                break;
            case 4://첫 몬스터 발견
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "헉, 몬스터가 있는 것 같아!",
                    "몬스터에게 다가가서 클릭하면\n타겟을 지정할 수 있어.",
                    "타겟을 지정한 뒤 숫자키 1을 눌러\n공격하자!",
                };
                break;
            case 5://첫 음식
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "으.. 뭔가를 먹으면 빨리 회복할 수\n있을 것 같아.",
                    "이 근처에 먹을 수 있는 게 없을까?",
                    "음식을 발견하면 클릭해보자!",
                };
                break;
            case 6:
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "이 울타리는 좀 아플 것 같아..",
                };
                goMainProgress = false;
                break;
            case 7:
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "울타리가 부서질 때까지 숫자키 1번을\n눌러보자!",
                };
                goMainProgress = false;
                break;
            case 8://울타리 부순 뒤
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "울타리가 부서졌다!",
                    "이제 앞으로 나아갈 수 있겠어.",
                    "네가 앞으로 가면 따라갈게.",
                    "좋아, 가자~!",
                };
                break;
            case 9://3렙에서 새 기술 습득
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "어쩐지 좀 더 강해진 것 같다!",
                    "이제부터 숫자키 2번으로\n새로운 공격을 할 수 있어!",
                    "이 기술은 한 번 쓰면 2초 후에\n다시 쓸 수 있다는 것도 명심해!",
                };
                break;
            case 10://버섯몬스터까지 다 해치움
                sentences = new string[] {
                    "..잠깐 기다려!",
                    "응?",
                    "..버섯 몬스터?",
                    "널 공격할 생각은 없으니까 걱정 마.\n그리고 난 '머시'라는 이름이 있다고.",//3
                    "너, 그 엘릭서인가 뭔가 하는 걸\n찾고 있는 거지?",
                    "맞아! 엘릭서에 대해 알아?",//5
                    "적어도 누가 가지고 있는 지는 알지.",
                    "아주 무시무시한 놈이야..\n그 놈이 숲에 오고 나서부터\n모든 게 달라졌지.",
                    "우리는 그 놈에게 협박당해서 어쩔 수 없이\n숲에 들어온 이들을 공격해야만 했어.",
                    "비록 네가 내 친구들을 다치게 하긴 했지만..\n너라면 그 놈을 상대할 수 있을 지도 몰라!",
                    "그래서 말인데, 나도 같이 싸울 수 있게\n해줘!",
                    "으음..",//11
                    "그래!",
                    "뭐, 정말?",
                    "내 이름은 포포야!\n여기 빨간 친구는 라임이고!",//14
                    "고마워! 도움이 되도록 노력할게!",
                };
                break;
            case 11://문지기 발견
                sentences = new string[] {
                    "앗, 저건?",//포포
                    "..강해 보이는군.",//라임
                    "저 녀석은 엘릭서를 가진 그 놈을\n지키는 문지기야.",//머시
                    "놈은 저 문 뒤에 있을 거야.",
                    "흐음, 아무래도 문을 그냥 열어줄 것 같진\n않은데..",//포포
                    "그렇다면 쟤를 쓰러뜨리고 문을\n여는 수 밖에 없겠네!",
                    "만만한 상대가 아니니 각오하는 게\n좋을 거야.",//머시
                };
                break;
            case 12://문지기 해치우고 5렙에서 새 기술 습득 + 캐릭터 간 대화
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "좀 더 강해진 것 같다!",
                    "이제부터 숫자키 3번으로\n새로운 공격을 할 수 있어!",
                     "이 기술은 한 번 쓰면 5초 후에\n다시 쓸 수 있다는 것도 명심하자!",
                    "해치웠다!",//포포 2
                    "쉽지 않은 상대였어.",//라임
                    "앗, 문이 열린다!",//포포 4
                    "드디어 그 놈과 마주하는 건가..!",//머시
                    "우리 셋이 함께라면 할 수 있어!",//포포 6
                    "가자~!!",
                };
                break;
            case 13://보스 쓰러뜨림
                sentences = new string[] {
                    "드디어 끝난 건가..",//라임
                    "놈을 쓰러뜨리다니..\n우린 이제 자유야!",//머시
                    "정말 고마워!",
                    "다 너희가 도와준 덕분이지, 뭐.",//포포 3
                    "앗, 저건..!",
                    "혹시 저 안에 엘릭서가..?",//머시
                    "어서 열어보라구.",//라임
                };
                break;
            case 14://엘릭서 획득
                NameTag.gameObject.SetActive(false);
                sentences = new string[] {
                    "엘릭서를 획득했다!",                  
                };
                break;
            case 15://엘릭서 획득 후
                sentences = new string[] {
                    "..이걸로 해산인가.",//라임
                    "그러게..",//머시 
                    "짧은 시간이었지만 그래도 너희와\n함께해서 즐거웠어.",
                    "너희는 나의 은인이야!",
                    "..덕분에 나도 울타리 너머에\n뭐가 있는지 알게 되었다.",//라임 4
                    "..앞으로 더 넓은 세상을 보고 싶은 마음도 생겼고.",
                    "나도 너희들 덕분에 엘릭서를\n얻을 수 있었어!",//포포 6
                    "고마워, 얘들아.",
                    "그래, 그럼 또 만날 수 있길\n기대하마, 꼬맹이.",//라임 8
                    "나중에 놀러오게 되면 내가 제대로\n대접할게! 그 때까지 잘 지내!",//머시
                };
                break;
            case 16://동료들 간 뒤
                sentences = new string[] {
                    "이제 이 엘릭서로 엄마를 치료할 수 있어!",
                    "여기까지 올 수 있었던 건\n전부 네 덕분이야!",
                    "정말 고맙다는 말 하고 싶고.. 또..",
                    "이런 모험이 살짝 그리울 것 같아!",
                    "..그럼 안녕! 나중에 또 보자!",
                    "게임 진행상황을 초기화하고 싶다면\n오른쪽 상단의 리셋버튼을 눌러주세요!\n초기화 시 게임이 자동으로 종료됩니다.",//5
                };
                break;
        }
    }

    void CheckDialogueEvent()
    {
        switch(dialogueNum)
        {
            case 1:
                NameTag.gameObject.SetActive(false);
                break;
            case 2://울타리 부술 때
                switch(index)
                {
                    case 1:
                        wait = true;
                        NameTag.gameObject.SetActive(true);//슬라임 등장
                        Name.text = "??";
                        NameTag.color = slimeColor;
                        Invoke(nameof(CanPrintNextDialogue), 2f);
                        break;
                    case 2:
                        wait = true;
                        CameraController.instance.SetFixedState(false);
                        CameraController.instance.SetYAxisValue(0.6f);
                        CameraController.instance.SetXAxisValue(-15f);
                        Player.instance.transform.LookAt(GameDirector.instance.friend_slime.transform);//슬라임 쳐다보기
                        ChangeToFoxNameTag();
                        Invoke(nameof(CanPrintNextDialogue), 1f);
                        break;
                    case 4:
                    case 6:
                    case 8:
                    case 11:
                        ChangeToFoxNameTag();
                        break;
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                        Name.text = "??";
                        NameTag.color = slimeColor;
                        break;
                    case 10:
                    case 12:
                        ChangeToSlimeNameTag();
                        break;
                    case 13:
                        NameTag.gameObject.SetActive(false);
                        break;
                }
                break;
            case 8://울타리 부순 뒤 대화
                switch (index)
                {
                    case 1:
                        NameTag.gameObject.SetActive(true);//슬라임 
                        ChangeToSlimeNameTag();
                        break;
                    case 3:  
                        ChangeToFoxNameTag();
                        break;         
                }
                break;
            case 10://두 번째 동료 영입
                switch (index)
                {
                    case 0:
                        wait = true;
                        NameTag.gameObject.SetActive(true);//버섯 등장
                        Name.text = "??";
                        NameTag.color = mushroomColor;
                        Invoke(nameof(CanPrintNextDialogue), 2f);
                        break;
                    case 1:
                        wait = true;
                        CameraController.instance.SetFixedState(false);
                        CameraController.instance.SetYAxisValue(0.6f);
                        CameraController.instance.SetXAxisValue(-15f);
                        Player.instance.transform.LookAt(GameDirector.instance.friend_mushroom.transform);//버섯 쳐다보기
                        GameDirector.instance.friend_slime.transform.LookAt(GameDirector.instance.friend_mushroom.transform);
                        ChangeToFoxNameTag();
                        Invoke(nameof(CanPrintNextDialogue), 1f);
                        break;
                    case 3:
                    case 6:
                    case 13:
                    case 15:
                        ChangeToMushroomNameTag();
                        break;
                    case 5:
                    case 11:
                    case 14:
                        ChangeToFoxNameTag();
                        break;
                }
                break;
            case 11://문지기 발견
                switch (index)
                {
                    case 0:
                    case 4:
                        NameTag.gameObject.SetActive(true);//포포 
                        ChangeToFoxNameTag();
                        break;
                    case 1:
                        ChangeToSlimeNameTag();
                        break;
                    case 2:
                    case 6:
                        ChangeToMushroomNameTag();
                        break;
                }
                break;
            case 12://새 기술 습득 + 문 열림
                switch (index)
                {
                    case 0:
                        //GameDirector.instance.gkBgm.wait();//문지기 bgm 중지
                        break;
                    case 3:
                    case 5:
                    case 7:
                        NameTag.gameObject.SetActive(true);//포포 
                        ChangeToFoxNameTag();
                        if(index == 5)
                        {
                            SoundManager.instance.PlayBossBgm();//보스 음악 재생
                        }
                        break;
                    case 4:
                        wait = true;
                        CameraController.instance.SetFixedState(false);
                        ////GameDirector.instance.ThirdPersonCamera.SetActive(true);
                        CameraController.instance.SetFixedState(false);//카메라 확대축소 가능
                        ChangeToSlimeNameTag();
                        GameDirector.instance.bossGate.SetTrigger("Open");//문 열리는 애니메이션
                        Invoke(nameof(CanPrintNextDialogue), 0.8f);
                        break;
                    case 6:
                        ChangeToMushroomNameTag();
                        break;
                }
                break;
            case 13://보스 해치움
                switch (index)
                {
                    case 0:
                    case 6:
                        if(index == 0)
                        {
                            SoundManager.instance.PlayEndingBgm();//음악 변경
                            NameTag.gameObject.SetActive(true);
                            ////GameDirector.instance.ThirdPersonCamera.SetActive(true);
                            CameraController.instance.SetFixedState(false);//카메라 확대축소 가능
                        }
                        ChangeToSlimeNameTag();
                        break;
                    case 1:
                    case 5:
                        ChangeToMushroomNameTag();
                        break;
                    case 3:
                        wait = true;
                        ChangeToFoxNameTag();
                        GameDirector.instance.treasureBox.SetActive(true);
                        GameDirector.instance.treasureBox.GetComponent<Animator>().SetTrigger("Down");//보물상자 떨어짐
                        GameDirector.instance.treasureBox.GetComponent<BoxCollider>().enabled = true; //(동료 캐릭터 움직임에 방해될까봐)꺼둔 콜라이더 활성화
                        Invoke(nameof(CanPrintNextDialogue), 1f);
                        break;
                    case 4:
                        Player.instance.transform.LookAt(GameDirector.instance.treasureBox.transform);//보물상자 쳐다보기
                        GameDirector.instance.friend_mushroom.transform.LookAt(GameDirector.instance.treasureBox.transform);
                        GameDirector.instance.friend_slime.transform.LookAt(GameDirector.instance.treasureBox.transform);
                        break;
                }
                break;
            case 15://엘릭서 획득 후
                switch (index)
                {
                    case 6:
                        NameTag.gameObject.SetActive(true);//포포 
                        ChangeToFoxNameTag();
                        break;
                    case 0:
                    case 4:
                    case 8:
                        NameTag.gameObject.SetActive(true);//포포 
                        ChangeToSlimeNameTag();
                        break;
                    case 1:
                    case 9:
                        ChangeToMushroomNameTag();
                        break;
                }
                break;
            case 16://동료들과 헤어진 후
                switch (index)
                {
                    case 0:
                        ChangeToFoxNameTag();
                        break;
                    case 5:
                        NameTag.gameObject.SetActive(false);
                        GameDirector.instance.resetButton.SetActive(true);//리셋버튼 활성화
                        break;
                }
                break;
        }
    }

    void ChangeToFoxNameTag()
    {
        Name.text = "포포";
        NameTag.color = new Color(255/ 255f, 121 / 255f, 0 / 255f);
    }

    void ChangeToSlimeNameTag()
    {
        Name.text = "라임";
        NameTag.color = slimeColor;
    }

    void ChangeToMushroomNameTag()
    {
        Name.text = "머시";
        NameTag.color = mushroomColor;
    }

    void CanPrintNextDialogue() //다음 대사로 넘어갈 수 있음
    {
        wait = false;
    }
}
