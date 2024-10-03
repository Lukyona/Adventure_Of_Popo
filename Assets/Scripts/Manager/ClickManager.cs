using UnityEngine;
using System.Text.RegularExpressions;

public class ClickManager : MonoBehaviour
{
    private int GetNumber(string name)
    {                   // Regex.Match : 정규 표현식을 사용, 문자열에서 특정 패턴을 찾아주는 기능
        return int.Parse(Regex.Match(name, @"\d+").Value); // \d+ : 하나 이상의 숫자를 의미 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.IsTalking)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 20f))
            {
                string objectName = hit.transform.gameObject.name;
                GameObject hittedObject = hit.collider.gameObject;

                HandleFenceClick(objectName);
                HandleFoodClick(objectName, hittedObject);
                HandleEnemyClick(hittedObject);
                HandleTreasureClick(hittedObject);
            }

        }
    }

    private void HandleFenceClick(string objectName)
    {
        if (GameManager.instance.MainCount > 5) return;

        if (objectName.Contains("Fence"))//펜스를 클릭하면
        {
            if (objectName.Contains("Destroyable"))
                GameManager.instance.isTargetFenceClicked = true;

            GameManager.instance.ClickFence();
        }
    }

    private void HandleFoodClick(string objectName, GameObject hittedObject)
    {
        if (objectName.Contains("food"))//음식을 클릭하면
        {
            int foodNum = GetNumber(objectName);
            DataManager.instance.EatFood(foodNum);

            Destroy(hittedObject);//먹은 음식 삭제                   
        }
    }

    private void HandleEnemyClick(GameObject hittedObject)
    {
        string objectTag = hittedObject.tag;

        if (objectTag == "Enemy")
        {//범위 11, 클릭한 오브젝트 태그가 몬스터이고 몬스터가 플레이어 시야 내에 있을 때, 플레이어가 죽은 상태가 아닐 때
            if (Player.instance.CombatComponent.EnemyInTargetRange && !Player.instance.IsDead())
            {
                if (hittedObject.GetComponent<IEnemyController>().IsDead()) return;

                if (Player.instance.GetTarget() != null)//타겟 범위 내에 기존 타겟이 있던 상태면
                {
                    EnemyHUD.instance.DisappearEnemyInfo();//기존 타겟 정보 안 보이게 하기
                }
                Player.instance.SetTarget(hittedObject);//타겟 할당
            }
        }
        else if (Player.instance.GetTarget() != null)//타겟이 설정된 상태에서 클릭한 게 몬스터가 아니면
        {
            Player.instance.SetTarget(null);
            EnemyHUD.instance.DisappearEnemyInfo();
        }
    }

    private void HandleTreasureClick(GameObject hittedObject)
    {
        if (GameManager.instance.MainCount < 11) return;

        string objectTag = hittedObject.tag;

        if (objectTag == "Treasure" && GameManager.instance.MainCount == 11)//보물상자 클릭
        {
            GameManager.instance.TreasureBox.tag = "Untagged";//태그 변경
            GameManager.instance.TreasureBox.GetComponent<Animator>().SetTrigger("Open");
            MyTaskManager.instance.ExecuteAfterDelay(() => GameManager.instance.TreasureBox.tag = "Elixir", 2f);
        }
        if (objectTag == "Elixir")//엘릭서 클릭
        {
            GameManager.instance.TreasureBox.tag = "Untagged";//태그 변경
            SoundManager.instance.PlayClickSound();
            Destroy(hittedObject.transform.Find("Elixir").gameObject);//오브젝트 파괴
            DialogueManager.instance.SetDialogue(14);
            GameManager.instance.StartTalk();
        }
    }

}

