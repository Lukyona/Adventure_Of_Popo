using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] GameObject black;
    Animator animator;

    [SerializeField] Texture2D cursorImg;

    bool start = false; //게임 시작 버튼 누르면 true

    private void Start()
    {
        animator = black.GetComponent<Animator>();
        Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);
        Invoke(nameof(Black_Off), 2.2f);
    }

    public void FadeToScene() // 게임 시작 버튼 클릭 시 실행
    {
        if(!start)
        {
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
