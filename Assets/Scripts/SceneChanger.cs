using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public GameObject black;
    bool start = false; //게임 시작 버튼 누르면 true

    private void Start()
    {
        Invoke(nameof(Black_Off), 2.2f);
    }

    public AudioSource click;
    public void FadeToScene()
    {
        if(!start)
        {
            click.Play();
            start = true;
            black.SetActive(true);
            animator.SetTrigger("BlackOn");
            Invoke(nameof(OnFadeComplete), 2.2f);
        }
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(1);//게임씬 로드
    }

    void Black_Off()
    {
        black.SetActive(false);
    }
}
