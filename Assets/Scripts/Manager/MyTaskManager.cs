using System.Collections;
using UnityEngine;
using System;

public class MyTaskManager : MonoBehaviour
{
    private static MyTaskManager _instance; // 프라이빗 필드

    public static MyTaskManager instance // 퍼블릭 프로퍼티
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("MyTaskManager");
                _instance = obj.AddComponent<MyTaskManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public Coroutine StartMyCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void StopMyCoroutine(IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
    }

    public void ExecuteAfterDelay(Action action, float delay) // Invoke 대신 사용
    {
        StartCoroutine(ExecuteAfterDelayCoroutine(action, delay));
    }

    private IEnumerator ExecuteAfterDelayCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke(); // 액션 호출
    }
}
