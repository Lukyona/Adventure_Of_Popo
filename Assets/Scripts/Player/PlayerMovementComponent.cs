using UnityEngine;
using Cinemachine;

public class PlayerMovementComponent
{
    private const float DEFAULT_SPEED = 4f;
    private const float RUN_SPEED = 5.5f;
    private const float GRAVITY = -30.5f;
    private const float JUMP_HEIGHT = 2.2f;

    private CharacterController playerController;
    private Transform cameraTransform;//메인 캠

    private Transform playerTransform;
    private Animator animator;

    private float trunSmoothTime = 0.1f;
    private float trunSmoothVelocity;

    private Vector3 velocity;
    Transform groundCheck;
    private float groundDistance = 0.4f;
    private bool isGrounded;

    private KeyCode jumpKey = KeyCode.Space; //점프키 설정
    private bool isJumping = false;
    public bool IsRunning { get; private set; }

    public bool Enabled { get; set; }

    public void Start()
    {
        Enabled = true;
        playerController = Player.instance.GetComponent<CharacterController>();
        playerTransform = Player.instance.transform;
        animator = Player.instance.Animator;
        cameraTransform = Camera.main.transform;
        CinemachineCore.GetInputAxis = GetAxisCustom;
        groundCheck = Player.instance.transform.Find("Ground Check");
    }

    private float GetAxisCustom(string axisName) //카메라 회전
    {
        if (Input.GetMouseButton(1))
        {
            float v = UnityEngine.Input.GetAxis(axisName);
            return -v;
        }
        else
        {
            return 0;
        }
    }

    private bool IsShiftPressed()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private bool IsMovementKeyPressed()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S);
    }

    public void Update()
    {
        if (!Enabled) return;

        UpdateGroundStatus();
        HandleJump();
        HandleMovement();

        velocity.y += GRAVITY * Time.deltaTime; //중력 적용

        playerController.Move(velocity * Time.deltaTime);
    }

    private void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, LayerMask.GetMask("Ground"));
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleJump()
    {
        if (!isJumping && Input.GetKeyDown(jumpKey) && isGrounded)
        {
            StartJump();
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isShiftPressed = IsShiftPressed();
        bool isMovementKeyPressed = IsMovementKeyPressed();

        if (direction.magnitude >= 0.1f)
        {
            ResetAnimatorStates();

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref trunSmoothVelocity, trunSmoothTime);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            playerTransform.rotation = Quaternion.Euler(0f, angle, 0f);

            float speed = isShiftPressed ? RUN_SPEED : DEFAULT_SPEED;
            playerController.Move(moveDir.normalized * speed * Time.deltaTime);
        }


        if (isShiftPressed)//쉬프트키를 누른 상태로
        {
            if (!isJumping && Player.instance.StatusComponent.CurrentStamina > 0 && isMovementKeyPressed)//뛰기
            {
                SetAnimatorState("Run", true);
                StartRunning();
            }
            else if (isMovementKeyPressed)//스태미나 없는데 쉬프트키와 이동키 누르면 그냥 걷기
            {
                if (isJumping) StopRunning();

                if (!isJumping)
                {
                    SetAnimatorState("Walk", true);
                }
            }
            else
            {
                if (isJumping) StopRunning();

                DontMove();
            }
        }
        else if (isMovementKeyPressed)
        {// 쉬프트키 없이 이동조작키 누르면 그냥 걷기
            if (!isJumping)
            {
                StopRunning();
                SetAnimatorState("Walk", true);
            }
        }
        else//이동하지 않는 상태
        {
            DontMove();
        }
    }

    private void SetAnimatorState(string state, bool value)
    {
        animator.SetBool(state, value);
    }

    private void ResetAnimatorStates()
    {
        SetAnimatorState("Walk", false);
        SetAnimatorState("Run", false);
        SetAnimatorState("Stop", false);
    }

    private void StartRunning()
    {
        IsRunning = true;
        MyTaskManager.instance.StartMyCoroutine(Player.instance.StatusComponent.ConsumeStamina());
    }
    private void StopRunning()
    {
        IsRunning = false;
        MyTaskManager.instance.StopMyCoroutine(Player.instance.StatusComponent.ConsumeStamina());
    }

    public void DontMove()
    {
        IsRunning = false;

        if (!isJumping)
        {
            ResetAnimatorStates();
            SetAnimatorState("Stop", true);
        }
    }

    private void StartJump()
    {
        isJumping = true;
        animator.SetTrigger("Jump");
        MyTaskManager.instance.ExecuteAfterDelay(Jump, 0.3f);
    }
    private void Jump()
    {
        velocity.y = Mathf.Sqrt(JUMP_HEIGHT * -2f * GRAVITY);
        MyTaskManager.instance.ExecuteAfterDelay(FinishJump, 0.9f);
    }
    private void FinishJump()
    {
        isJumping = false;
    }
}
