using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float alphaSpeed;
    Text text;
    Color alpha;

    public float destroyTime;
    public int damage;

    void Start()
    {
        text = GetComponent<Text>();
        text.text = damage.ToString();
        alpha = text.color;
        Invoke(nameof(DestroyObject), destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(new Vector3(0, 200 * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
