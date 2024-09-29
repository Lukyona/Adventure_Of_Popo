using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour // 몬스터/플레이어 데미지 텍스트 프리팹에 추가되어있음
{
    public float alphaSpeed;
    public float destroyTime;
    private float damage;
    private Text text;
    private Color alpha;

    private void Start()
    {
        text = GetComponent<Text>();
        text.text = damage.ToString();
        alpha = text.color;
        Invoke(nameof(DestroyObject), destroyTime);
    }

    private void Update()
    {
        gameObject.transform.Translate(new Vector3(0, 200 * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    public void SetDamage(float value)
    {
        damage = value;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
