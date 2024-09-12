using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerStatusComponent
{
    public float CurrentHealth {get; set;} = 100;
    public float MaxHealth {get; private set;} = 100;
    public bool IsHealthRecovering {get; private set;}

    public float CurrentStamina {get; private set;} = 12;
    public float MaxStamina {get; private set;} = 12;
    public bool IsStaminaRecovering {get; private set;}

    public int CurrentLevel {get; private set;} = 1;

    public float CurrentExp {get; private set;} = 0;
    public float MaxExp {get; private set;} = 50;

    private Timer timer;
    private Timer healthTimer;
    private Timer staminaTimer;

    void Update()
    {
        if(CurrentHealth > 0)
        {
            if(CurrentStamina < MaxStamina && !IsStaminaRecovering && !ThirdPlayerMovement.instance.running)
            {//현재 스태미나가 스태미나 최대치 미만이고 스태미나 회복 중이 아니고 달리지 않을 때
                IsStaminaRecovering = true;
                StartStaminaRecovery();
            }
            
            if(CurrentHealth < MaxHealth && !IsHealthRecovering)
            {
                IsHealthRecovering = true;
                StartStaminaRecovery();
            }
        }
        else
        {
            IsHealthRecovering = false;
            IsStaminaRecovering = false;
            healthTimer.Dispose();
            staminaTimer.Dispose();
        }
    }

    void StartHealthRecovery()
    {
        int period = 0; // 단위는 ms, 1000 = 1초
        switch(CurrentLevel)//렙마다 회복속도 차이남
        {
            case 1:
                period = 2000;
                if (!ThirdPlayerMovement.instance.monsterInTargetRange && GameDirector.instance.mainCount == 3 && !GameDirector.instance.talking)//생명력 깎인 뒤 과일에 대한 설명
                {
                    DialogueController.instance.SetDialogue(5);
                    GameDirector.instance.Fox_Cant_Move(); //플레이어 이동, 카메라 회전 금지
                    ThirdPlayerMovement.instance.DontMove();
                    GameDirector.instance.Start_Talk();
                }
                break;
            case 2:
                period = 1800;
                break;
            case 3:
                period = 1500;
                break;
            case 4:
                period = 1300;
                break;
            case 5:
                period = 1000;
                break;
        }

        healthTimer = new Timer(_ => RecoverHealth(), null, 1000, period); // 처음만 1초 지연, 나머지는 period만큼 지연
    }

    void RecoverHealth()//스태미나 회복
    {
        if(CurrentHealth == MaxHealth)
        {
            IsHealthRecovering = false;
            healthTimer.Dispose();
        }
        else
        {
            CurrentHealth += 1;
            UIManager.instance.UpdatePlayerHealthUI();         
        }
    }

    void StartStaminaRecovery()
    {
        int period = 0; // 단위는 ms, 1000 = 1초
        switch(CurrentLevel)//렙마다 회복속도 차이남
        {
            case 1:
                period = 1000;
                break;
            case 2:
                period = 750;
                break;
            case 3:
                period = 500;
                break;
            case 4:
                period = 400;
                break;
            case 5:
                period = 300;
                break;
        }

        staminaTimer = new Timer(_ => RecoverStamina(), null, 300, period);
    }

    void RecoverStamina()//스태미나 회복
    {
        if(CurrentStamina == MaxStamina)
        {
            IsStaminaRecovering = false;
            staminaTimer.Dispose();
        }
        else
        {
            CurrentStamina += 1;
            UIManager.instance.UpdatePlayerStaminaUI();         
        }
    }

    public void ModifyHealth(float value)
    {
        CurrentHealth += value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        UIManager.instance.UpdatePlayerHealthUI(); 

        if (CurrentHealth <= 0)//생명력이 0이하일 땐
        {
            GameDirector.instance.Fox_Cant_Move();//플레이어 이동 금지
            ThirdPlayerMovement.instance.foxAnimator.SetTrigger("Die");//쓰러짐 애니메이션

            if(Player.instance.GetTarget() != null)
            {
                Player.instance.SetTarget(null);
            }
            
            // 타이머 사용해서 2초 후에 함수 호출
            timer = new Timer(_ => UIManager.instance.StartBlackOut(), null, 2000, Timeout.Infinite);
        }       
    }

    public IEnumerator ConsumeStamina()
    {
        CurrentStamina -= 1;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina); // 스태미나가 음수가 되지 않도록 제한
        UIManager.instance.UpdatePlayerStaminaUI();

        yield return new WaitForSeconds(0.07f);
    }

    public void DisposeTimer()
    {
        timer.Dispose();
    }

    public void GetEXP(string enemyName)//경험치 얻는 함수
    {
        switch(enemyName)
        {
            case "Slime":
            case "Slime2":
                CurrentExp += 5;
                break;
            case "Turtle":
                CurrentExp += 10;
                break;
            case "Log":
                CurrentExp += 20;
                break;
            case "Bat":
                CurrentExp += 25;
                break;
            case "Mushroom":
                CurrentExp += 33;
                break;
            case "DogKnight":
                CurrentExp += 48;
                break;
        }

        if(GameDirector.instance.mainCount == 7)
        {
            if (DataManager.instance.AliveMonsters["Log"] == 0 && DataManager.instance.AliveMonsters["Bat"] == 0 && DataManager.instance.AliveMonsters["Mushroom"] == 0)
            {
                UIManager.instance.ActiveBlackScreen();
                GameDirector.instance.Fox_Cant_Move();
                GameDirector.instance.Invoke(nameof(GameDirector.instance.Ready_To_Talk), 2.2f);
                DialogueController.instance.SetDialogue(10);
            }
        }

        UIManager.instance.UpdatePlayerExpUI();

        if (CurrentExp >= MaxExp && CurrentLevel != 5)//레벨업, 5레벨 아닐 때만 가능
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        SoundManager.instance.PlayLevelUpSound();
        
        CurrentExp -= MaxExp; //현재 경험치 
        CurrentLevel++;

        switch (CurrentLevel)
        {
            case 2:
                MaxHealth = 150;
                MaxStamina = 14;
                MaxExp = 80;
                if(GameDirector.instance.mainCount == 4)
                {
                    MonsterHPBar.instance.Fence_arrow();
                }
                break;
            case 3:
                MaxHealth = 200;
                MaxStamina = 16;
                MaxExp = 120;
                break;
            case 4:
                MaxHealth = 250;
                MaxStamina = 18;
                MaxExp = 160;
                break;
            case 5:
                MaxHealth = 300;
                MaxStamina = 20;
                break;
        }
        
        CurrentHealth = MaxExp;
        CurrentStamina = MaxStamina;

        UIManager.instance.UpdatePlayerHealthUI();
        UIManager.instance.UpdatePlayerStaminaUI();
        UIManager.instance.UpdatePlayerLevelUI();
    }
   
    public void SavePlayerStatus()
    {
        PlayerPrefs.SetInt("Level", CurrentLevel);
        PlayerPrefs.SetFloat("EXP", CurrentExp);
        PlayerPrefs.SetFloat("HP", CurrentHealth);
        PlayerPrefs.SetFloat("SP", CurrentStamina);
    }

    public void LoadPlayerStatus()
    {
        CurrentLevel = PlayerPrefs.GetInt("Level");
        CurrentExp = PlayerPrefs.GetFloat("EXP");
        CurrentHealth = PlayerPrefs.GetFloat("HP");
        CurrentStamina = PlayerPrefs.GetFloat("SP");

        UIManager.instance.UpdatePlayerLevelUI();
        UIManager.instance.UpdatePlayerExpUI();
        UIManager.instance.UpdatePlayerHealthUI();
        UIManager.instance.UpdatePlayerStaminaUI();
    }
}
