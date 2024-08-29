using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    #region 
    public GameObject target;
    bool canAttack1 = false; //공격 텀을 위한 변수
    bool canAttack2 = false;
    bool canAttack3 = false;
    int fenceHitCount = 0;//펜스 공격한 횟수
    int attackNum = 0;// 몇번 공격인지 구분
    #endregion
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
