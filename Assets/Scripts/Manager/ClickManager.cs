using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    int GetNumber(string name, int nameSize)
    {
        int lastIdx = nameSize - 1;
        // 숫자가 두 자릿수인지 확인
        int i = 0;
        bool isTwoDigit = int.TryParse(name.Substring(lastIdx-1,1), out i); // int형에 속하는지 확인, 
        //TryParse : 숫자가 아닌 문자를 포함하거나 지정한 형식에 비해 너무 크거나 작은 경우 false 반환, out 매개 변수를 0으로 설정
        //그렇지 않으면 true 반환 out 매개 변수를 문자열의 숫자 값으로 설정
        
        if(isTwoDigit) 
            return int.Parse(name.Substring(lastIdx-1,2)); // 두 자리 숫자 추출
        else
            return int.Parse(name.Substring(lastIdx,1)); // 한 자리 숫자만 추출
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 11f))//범위 11
            {
                string objectName = hit.transform.gameObject.name;
                if(objectName == "Fence_Destroyable")//펜스를 클릭하면
                {
                    GameDirector.instance.can_hit = true;
                    GameDirector.instance.ClickFence();
                }
                if (objectName.Contains("food"))//음식을 클릭하면
                {
                    int foodNum = GetNumber(objectName, objectName.Length);
                    DataManager.instance.EatFood(foodNum);
                    
                    Destroy(hit.transform.gameObject);//먹은 음식 삭제                   
                }
            }

            if (Physics.Raycast(ray, out hit, 11f) && hit.collider.gameObject.tag == "Enemy" && ThirdPlayerMovement.instance.monsterInTargetRange && !Player.instance.IsDead())
            {//범위 11, 클릭한 오브젝트 태그가 몬스터이고 몬스터가 플레이어 시야 내에 있을 때, 플레이어가 죽은 상태가 아닐 때
                GameObject targetObj = hit.collider.gameObject;//타겟에 클릭한 오브젝트 할당
                if(targetObj.GetComponent<IEnemyController>().IsDead()) return;
                
                if (MonsterHPBar.instance.targetIn)//타겟 범위 내에 기존 타겟이 있던 상태면
                {
                    MonsterHPBar.instance.DisappearMonsterInfo();//기존 타겟 정보 안 보이게 하기
                }
                Player.instance.SetTarget(targetObj);//타겟 할당
            }
            if(Physics.Raycast(ray, out hit, 20f) && !(hit.collider.gameObject.tag == "Enemy") && Player.instance.GetTarget() != null)//타겟이 설정된 상태에서 클릭한 게 몬스터가 아니면
            {
                Player.instance.SetTarget(null);
                MonsterHPBar.instance.DisappearMonsterInfo();
            }

            if (Physics.Raycast(ray, out hit, 10f) && hit.collider.gameObject.tag == "Treasure" && GameDirector.instance.mainCount == 11)//보물상자 클릭
            {
                GameDirector.instance.treasureBox.tag = "Untagged";//태그 변경
                GameDirector.instance.treasureBox.GetComponent<Animator>().SetTrigger("Open");
                GameDirector.instance.Invoke("CanGetElixir", 2f);//2초 후 엘리서 획득 가능
            }
            if (Physics.Raycast(ray, out hit, 10f) && hit.collider.gameObject.tag == "Elixir")//엘릭서 클릭
            {
                GameDirector.instance.treasureBox.tag = "Untagged";//태그 변경
                SoundManager.instance.PlayClickSound();
                Destroy(GameDirector.instance.elixir);//오브젝트 파괴
                DialogueController.instance.SetDialogue(14);
                GameDirector.instance.Start_Talk();
            }
        }
    }
}

