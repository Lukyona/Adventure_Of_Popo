using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageText : MonoBehaviour // 몬스터/플레이어 데미지 텍스트 프리팹에 추가되어있음
{
    public float alphaSpeed;
    public float destroyTime;
    int damage;
    Text text;
    Color alpha;

    void Start()
    {
        text = GetComponent<Text>();
        text.text = damage.ToString();
        alpha = text.color;
        Invoke(nameof(DestroyObject), destroyTime);
    }

    void Update()
    {
        gameObject.transform.Translate(new Vector3(0, 200 * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    public void SetDamage(int value)
    {
        damage = value;
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
