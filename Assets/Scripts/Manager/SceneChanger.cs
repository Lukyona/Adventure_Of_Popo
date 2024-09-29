using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject black;
    private Animator animator;

    [SerializeField] private Texture2D cursorImg;

    private bool start = false; //게임 시작 버튼 누르면 true

    private void Start()
    {
        animator = black.GetComponent<Animator>();
        Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);
        Invoke(nameof(Black_Off), 2.2f);
    }

    public void FadeToScene() // 게임 시작 버튼 클릭 시 실행
    {
        if (!start)
        {
            start = true;
            black.SetActive(true);
            animator.SetTrigger("BlackOn");
            Invoke(nameof(OnFadeComplete), 2.2f);
        }
    }

    private void OnFadeComplete()
    {
        SceneManager.LoadScene(1);//게임씬 로드
    }

    private void Black_Off()
    {
        black.SetActive(false);
    }
}
