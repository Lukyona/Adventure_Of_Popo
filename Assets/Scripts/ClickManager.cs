using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 11f))//범위 11
            {
                if(hit.transform.gameObject.name.Contains("Fence"))//펜스를 클릭하면
                {
                    if(hit.transform.gameObject.name.Contains("(1)"))
                    {
                        GameDirector.instance.can_hit = true;
                    }
                    GameDirector.instance.Fence_Level2();
                }
                if (hit.transform.gameObject.name.Contains("food"))//음식을 클릭하면
                {
                    GameObject food = hit.transform.gameObject;//클릭한 음식 할당
                    if(food.name.Contains("tomato"))//토마토면
                    {
                        PlayerInfoManager.instance.Eat_Food(0);
                    }
                    if(food.name.Contains("orange"))
                    {
                        PlayerInfoManager.instance.Eat_Food(1);
                    }
                    if (food.name == "kiwi_food")
                    {
                        PlayerInfoManager.instance.Eat_Food(2);
                    }
                    if (food.name == "kiwi1_food")
                    {
                        PlayerInfoManager.instance.Eat_Food(3);
                    }
                    if (food.name.Contains("carrot"))
                    {
                        PlayerInfoManager.instance.Eat_Food(4);
                    }
                    if (food.name.Contains("lemon"))
                    {
                        PlayerInfoManager.instance.Eat_Food(5);
                    }
                    if (food.name.Contains("cherry"))
                    {
                        PlayerInfoManager.instance.Eat_Food(6);
                    }
                    if (food.name.Contains("apple"))
                    {
                        PlayerInfoManager.instance.Eat_Food(7);
                    }
                    if (food.name.Contains("banana"))
                    {
                        PlayerInfoManager.instance.Eat_Food(8);
                    }
                    if (food.name == "acorn_food")
                    {
                        PlayerInfoManager.instance.Eat_Food(9);
                    }
                    if (food.name == "acorn1_food")
                    {
                        PlayerInfoManager.instance.Eat_Food(10);
                    }
                    if (food.name == "acorn2_food")
                    {
                        PlayerInfoManager.instance.Eat_Food(11);
                    }
                    Destroy(food);//먹은 음식 삭제                   
                }
            }

            if (Physics.Raycast(ray, out hit, 11f) && hit.collider.gameObject.tag == "Monster" && ThirdPlayerMovement.instance.monsterInTargetRange && !PlayerInfoManager.instance.death)
            {//범위 11, 클릭한 오브젝트 태그가 몬스터이고 몬스터가 플레이어 시야 내에 있을 때, 플레이어가 죽은 상태가 아닐 때
                PlayerAttack.instance.target = null;
                GameObject targetObj = hit.collider.gameObject;//타겟에 클릭한 오브젝트 할당
                if (MonsterHPBar.instance.targetIn)//타겟 범위 내에 기존 타겟이 있던 상태면
                {
                    MonsterHPBar.instance.DisappearMonsterInfo();//기존 타겟 정보 안 보이게 하기
                }
                PlayerAttack.instance.target = targetObj;//타겟 할당
                PlayerInfoManager.instance.monster = targetObj;
            }
            if(Physics.Raycast(ray, out hit, 20f) && !(hit.collider.gameObject.tag == "Monster") && PlayerAttack.instance.target != null)//타겟이 설정된 상태에서 클릭한 게 몬스터가 아니면
            {
                MonsterHPBar.instance.DisappearMonsterInfo();
                PlayerAttack.instance.target = null;
            }

            if (Physics.Raycast(ray, out hit, 10f) && hit.collider.gameObject.tag == "Treasure" && GameDirector.instance.mainCount == 11)//보물상자 클릭
            {
                GameDirector.instance.treasureBox.tag = "Untagged";//태그 변경
                GameDirector.instance.treasureBox.GetComponent<Animator>().SetTrigger("Open");
                Invoke(nameof(CanGetElixir), 2f);//2초 후 엘리서 획득 가능
            }
            if (Physics.Raycast(ray, out hit, 10f) && hit.collider.gameObject.tag == "Elixir")//엘릭서 클릭
            {
                GameDirector.instance.treasureBox.tag = "Untagged";//태그 변경
                GameDirector.instance.ClickSound();
                Destroy(GameDirector.instance.elixir);//오브젝트 파괴
                DialogueController.instance.DialogueSentences(14);
                GameDirector.instance.Start_Talk();
            }
        }
    }
    void CanGetElixir()
    {
        GameDirector.instance.treasureBox.tag = "Elixir";//태그 변경
    }
}

