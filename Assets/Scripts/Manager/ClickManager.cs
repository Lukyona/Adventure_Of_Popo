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
                if (objectName.Contains("Fence"))//펜스를 클릭하면
                {
                    if (objectName.Contains("Destroyable"))
                        GameManager.instance.isTargetFenceClicked = true;

                    GameManager.instance.ClickFence();
                }
                if (objectName.Contains("food"))//음식을 클릭하면
                {
                    int foodNum = GetNumber(objectName);
                    DataManager.instance.EatFood(foodNum);

                    Destroy(hit.transform.gameObject);//먹은 음식 삭제                   
                }

                if (hit.collider.gameObject.CompareTag("Enemy") && Player.instance.CombatComponent.EnemyInTargetRange && !Player.instance.IsDead())
                {//범위 11, 클릭한 오브젝트 태그가 몬스터이고 몬스터가 플레이어 시야 내에 있을 때, 플레이어가 죽은 상태가 아닐 때
                    GameObject targetObj = hit.collider.gameObject;//타겟에 클릭한 오브젝트 할당
                    if (targetObj.GetComponent<IEnemyController>().IsDead()) return;

                    if (Player.instance.GetTarget() != null)//타겟 범위 내에 기존 타겟이 있던 상태면
                    {
                        EnemyHUD.instance.DisappearEnemyInfo();//기존 타겟 정보 안 보이게 하기
                    }
                    Player.instance.SetTarget(targetObj);//타겟 할당
                }
                if (!hit.collider.gameObject.CompareTag("Enemy") && Player.instance.GetTarget() != null)//타겟이 설정된 상태에서 클릭한 게 몬스터가 아니면
                {
                    Player.instance.SetTarget(null);
                    EnemyHUD.instance.DisappearEnemyInfo();
                }

                if (hit.collider.gameObject.CompareTag("Treasure") && GameManager.instance.MainCount == 11)//보물상자 클릭
                {
                    GameManager.instance.TreasureBox.tag = "Untagged";//태그 변경
                    GameManager.instance.TreasureBox.GetComponent<Animator>().SetTrigger("Open");
                    GameManager.instance.Invoke("CanGetElixir", 2f);//2초 후 엘리서 획득 가능
                }
                if (hit.collider.gameObject.CompareTag("Elixir"))//엘릭서 클릭
                {
                    GameManager.instance.TreasureBox.tag = "Untagged";//태그 변경
                    SoundManager.instance.PlayClickSound();
                    Destroy(hit.collider.gameObject.transform.Find("Elixir").gameObject);//오브젝트 파괴
                    DialogueManager.instance.SetDialogue(14);
                    GameManager.instance.StartTalk();
                }
            }

        }
    }
}

