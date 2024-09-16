using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPlayerMovement : MonoBehaviour
{
    public static ThirdPlayerMovement instance;

    public CharacterController controller; //플레이어의 캐릭터 컨트롤러
    public CharacterController slime;//슬라임 컨트롤러
    public Transform cam;//메인 캠
    public Animator foxAnimator;

    public float speed = 4f; //플레이어 이동 속도
    public float trunSmoothTime = 0.1f;
    float trunSmoothVelocity;

    float gravity = -30.5f;
    private float jumpHeight = 2.2f; //점프 높이
    private KeyCode jumpKey = KeyCode.Space; //점프키 설정

    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;

    public bool jumping = false;

    public bool running = false;

    public LayerMask whatIsMonster;
    public float sightRange, attackRange, targetRange;//플레이어의 시야범위, 공격범위, 타겟 표시 범위
    public bool monsterInSightRange, monsterInAttackRange, monsterInTargetRange; //몬스터가 해당 범위 내에 있을 경우 true

    public LayerMask gk;//이 오브젝트가 범위 내에 있으면 문지기 발견

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    public float GetAxisCustom(string axisName) //카메라 회전
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                float v = UnityEngine.Input.GetAxis("Mouse X");
                return -v;
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                float v = UnityEngine.Input.GetAxis("Mouse Y");
                return -v;
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref trunSmoothVelocity, trunSmoothTime);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if(!((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.S)))//뒤로 가는 것 제외 방향 회전하여 이동
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }
        velocity.y += gravity * Time.deltaTime; //중력 적용

        if (!((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.S)))
        {
            controller.Move(velocity * Time.deltaTime);        
        }

        if (jumping == false && Input.GetKeyDown(jumpKey) && isGrounded)//점프 중이 아니고, 땅에 닿아있는 상태에서 점프키 누르면
        {
            foxAnimator.SetBool("WalkBack", false);
            foxAnimator.SetBool("Run", false);
            foxAnimator.SetBool("Walk", false);
            foxAnimator.SetBool("Stop", false);
            jumping = true;
            JumpTo();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))//쉬프트키를 누른 상태로
        {
            if (Input.GetKey(KeyCode.S))//뒤로 걷기
            {
                if(running) StopRunning();
                foxAnimator.SetBool("Stop", false);
                foxAnimator.SetBool("Walk", false);
                foxAnimator.SetBool("Run", false);
                foxAnimator.SetBool("WalkBack", true);
            }
            else if (jumping == false && Player.instance.StatusComponent.CurrentStamina > 0 && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))//뛰기
            {
                speed = 5.5f;//속도 증가
                foxAnimator.SetBool("Stop", false);
                foxAnimator.SetBool("Walk", false);
                foxAnimator.SetBool("WalkBack", false);
                foxAnimator.SetBool("Run", true);
                StartRunning();
            }
            else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))//스태미나 없는데 쉬프트키와 이동키 누르면 그냥 걷기
            {
                if(running) StopRunning();
                
                foxAnimator.SetBool("WalkBack", false);
                foxAnimator.SetBool("Run", false);
                foxAnimator.SetBool("Stop", false);
                if (jumping == false)
                {
                    speed = 4f;
                    foxAnimator.SetBool("Walk", true);
                }
            }
            else
            {
                if(running) StopRunning();
                DontMove();
            }
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
                    || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {// 쉬프트키 없이 이동조작키 누르면 그냥 걷기
            foxAnimator.SetBool("WalkBack", false);
            foxAnimator.SetBool("Run", false);
            foxAnimator.SetBool("Stop", false);
            if (jumping == false && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if(running) StopRunning();
                speed = 4f;
                foxAnimator.SetBool("Walk", true);
            }
        }
        else//이동하지 않는 상태
        {
            DontMove();
        }

        monsterInSightRange = Physics.CheckSphere(gameObject.transform.position, sightRange, whatIsMonster);
        monsterInAttackRange = Physics.CheckSphere(gameObject.transform.position, attackRange, whatIsMonster);
        monsterInTargetRange = Physics.CheckSphere(gameObject.transform.position, targetRange, whatIsMonster);

        if(monsterInSightRange)
        {
            //Debug.Log("시야에 몬스터");
            if(GameDirector.instance.mainCount == 2 && !GameDirector.instance.talking)//소개 후 첫 몬스터 발견
            {
                DialogueController.instance.SetDialogue(4);//몬스터 첫 발견
                DontMove();
                GameDirector.instance.Start_Talk();
            }      
        }
        if(monsterInAttackRange)
        {
            //Debug.Log("공격범위 내 몬스터");

        }
        if(!monsterInTargetRange && Player.instance.GetTarget())//타겟 몬스터가 타겟팅 범위 벗어나면
        {
            Player.instance.SetTarget(null); //타겟 해제
        }

        if(GameDirector.instance.mainCount == 8)
        {
            bool gateKeeperInRange = Physics.CheckSphere(gameObject.transform.position, sightRange, gk);//gk가 시야범위에 있으면 true
            if(gateKeeperInRange && !GameDirector.instance.talking)
            {
                DialogueController.instance.SetDialogue(11);
                GameDirector.instance.Start_Talk();
                CameraController.instance.SetFixedState(false);//카메라 확대축소 가능
            }
        }

    }

    void StartRunning()
    {
        running = true;
        StartCoroutine(Player.instance.StatusComponent.ConsumeStamina());
    }

    void StopRunning()
    {
        running = false;
        StopCoroutine(Player.instance.StatusComponent.ConsumeStamina());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; //공격범위 레드로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);

        Gizmos.color = Color.green; //시야범위 그린으로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, sightRange);

        Gizmos.color = Color.blue; //타겟범위 그린으로 표시
        Gizmos.DrawWireSphere(gameObject.transform.position, targetRange);
    }

    public bool isPlayerIdle = true;
    public bool isPlayerAction = false;
    public void DontMove()
    {
        isPlayerAction = false;
        isPlayerIdle = true;
        running = false;
        foxAnimator.SetBool("WalkBack", false);
        foxAnimator.SetBool("Run", false);
        foxAnimator.SetBool("Walk", false);
        if (jumping == false)
        {
            foxAnimator.SetBool("Stop", true);
        }
    }

    public void JumpTo()
    {
        foxAnimator.SetTrigger("Jump");
        Invoke(nameof(Jump), 0.3f);

    }
    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        Invoke(nameof(FinishJump), 0.9f);
    }

    void FinishJump()
    {
        jumping = false;
    }

}
