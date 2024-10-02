using System.Collections;
using UnityEngine;

public class PlayerStatusComponent
{
    public float CurrentHealth { get; set; } = 100;
    public float MaxHealth { get; private set; } = 100;
    public bool IsHealthRecovering { get; private set; } = false;
    private float healthRecoveryInterval = 1f;


    public float CurrentStamina { get; private set; } = 12;
    public float MaxStamina { get; private set; } = 12;
    private bool IsStaminaRecovering = false;
    private float staminaRecoveryInterval = 1f;
    private bool isStaminConsuming = false;

    public int CurrentLevel { get; private set; } = 1;

    public float CurrentExp { get; private set; } = 0;
    public float MaxExp { get; private set; } = 50;

    public void Update()
    {
        if (CurrentHealth > 0)
        {
            if (CurrentStamina < MaxStamina && !IsStaminaRecovering && !Player.instance.MovementComponent.IsRunning)
            {//현재 스태미나가 스태미나 최대치 미만이고 스태미나 회복 중이 아니고 달리지 않을 때
                MyTaskManager.instance.StartMyCoroutine(RecoverStamina());
            }

            if (CurrentHealth < MaxHealth && !IsHealthRecovering)
            {
                MyTaskManager.instance.StartMyCoroutine(RecoverHealth());
            }
        }
        else
        {
            IsHealthRecovering = false;
            IsStaminaRecovering = false;
        }
    }

    #region Health 관련
    private IEnumerator RecoverHealth()//스태미나 회복
    {
        if (IsHealthRecovering) yield break;

        IsHealthRecovering = true;

        switch (CurrentLevel)//렙마다 회복속도 차이남
        {
            case 1:
                healthRecoveryInterval = 2f;
                if (!Player.instance.CombatComponent.EnemyInTargetRange && GameManager.instance.MainCount == 3 && !GameManager.instance.IsTalking)//생명력 깎인 뒤 과일에 대한 설명
                {
                    DialogueManager.instance.SetDialogue(5);
                    Player.instance.DisableMovement(); //플레이어 이동, 카메라 회전 금지
                    Player.instance.MovementComponent.DontMove();
                    GameManager.instance.StartTalk();
                }
                break;
            case 2:
                healthRecoveryInterval = 1.8f;
                break;
            case 3:
                healthRecoveryInterval = 1.5f;
                break;
            case 4:
                healthRecoveryInterval = 1.3f;
                break;
            case 5:
                healthRecoveryInterval = 1f;
                break;
        }

        CurrentHealth += 1;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        UIManager.instance.UpdatePlayerHealthUI();

        yield return new WaitForSeconds(healthRecoveryInterval);
        IsHealthRecovering = false;
    }

    public void ModifyHealth(float value)
    {
        CurrentHealth += value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        UIManager.instance.UpdatePlayerHealthUI();

        if (CurrentHealth <= 0)//생명력이 0이하일 땐
        {
            Player.instance.Die();
        }
    }
    #endregion

    private IEnumerator RecoverStamina()//스태미나 회복
    {
        if (IsStaminaRecovering) yield break;

        IsStaminaRecovering = true;

        switch (CurrentLevel)
        {
            case 1:
                staminaRecoveryInterval = 1f;
                break;
            case 2:
                staminaRecoveryInterval = 0.75f;
                break;
            case 3:
                staminaRecoveryInterval = 0.5f;
                break;
            case 4:
                staminaRecoveryInterval = 0.4f;
                break;
            case 5:
                staminaRecoveryInterval = 0.3f;
                break;
        }

        CurrentStamina += 1;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina); // 스태미나가 음수가 되지 않도록 제한
        UIManager.instance.UpdatePlayerStaminaUI();

        yield return new WaitForSeconds(staminaRecoveryInterval);
        IsStaminaRecovering = false;
    }

    public IEnumerator ConsumeStamina()
    {
        if (isStaminConsuming) yield break;

        isStaminConsuming = true;

        CurrentStamina -= 1;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina); // 스태미나가 음수가 되지 않도록 제한
        UIManager.instance.UpdatePlayerStaminaUI();

        yield return new WaitForSeconds(0.07f);
        isStaminConsuming = false;
    }

    public void GetEXP(string enemyName)//경험치 얻는 함수
    {
        switch (enemyName)
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

        if (GameManager.instance.MainCount == 7)
        {
            if (DataManager.instance.AliveMonsters["Log"] == 0 && DataManager.instance.AliveMonsters["Bat"] == 0 && DataManager.instance.AliveMonsters["Mushroom"] == 0)
            {
                UIManager.instance.ActiveBlackScreen();
                Player.instance.DisableMovement();
                GameManager.instance.Invoke(nameof(GameManager.instance.ReadyToTalk), 2.2f);
                DialogueManager.instance.SetDialogue(10);
            }
        }

        if (CurrentExp >= MaxExp && CurrentLevel != 5)//레벨업, 5레벨 아닐 때만 가능
        {
            LevelUp();
        }

        UIManager.instance.UpdatePlayerExpUI();
    }

    private void LevelUp()
    {
        SoundManager.instance.PlayLevelUpSound();

        CurrentExp -= MaxExp; //현재 경험치 
        CurrentLevel++;

        SetMaxStatus();

        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;

        UIManager.instance.UpdatePlayerHealthUI();
        UIManager.instance.UpdatePlayerStaminaUI();
        UIManager.instance.UpdatePlayerLevelUI();
    }

    private void SetMaxStatus()
    {
        switch (CurrentLevel)
        {
            case 2:
                MaxHealth = 150;
                MaxStamina = 14;
                MaxExp = 80;
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

        SetMaxStatus();

        UIManager.instance.UpdatePlayerLevelUI();
        UIManager.instance.UpdatePlayerExpUI();
        UIManager.instance.UpdatePlayerHealthUI();
        UIManager.instance.UpdatePlayerStaminaUI();
    }
}
