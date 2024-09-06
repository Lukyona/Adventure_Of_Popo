using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawAttack : PlayerAttack
{
    int fenceHitCount = 0;//펜스 공격한 횟수

    public override void Execute(GameObject target)
    {
        if (target.name.Contains("Turtle"))
        {
            target.GetComponent<IEnemyController>().TakeDamage(12);
        }
        else
        {
            target.GetComponent<IEnemyController>().TakeDamage(15);
        }
        SoundManager.instance.PlayAttack1Sound();
        if(Physics.CheckSphere(GameDirector.instance.Player.transform.position, 5f, LayerMask.NameToLayer("Fence")) && GameDirector.instance.mainCount == 5)//펜스가 범위내에 있을 때
        {
            fenceHitCount++;
            if (fenceHitCount == 5)//공격횟수가 5일 때 한번만 발생
            {
                GameDirector.instance.HitWoodenFence();
            }
        }
    }

    public override float Cooldown => 0f; // 첫 번째 공격은 쿨타임이 없음, get하면 0f를 반환
}