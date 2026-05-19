using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("组件设置")]
    private CharacterController characterController;
    private Animator animator;
    public Transform followingCamera;
    public PlayerDataSO data;

    [Header("基础设置")]
    public float currentSpeed;
    public float gravity = 9.81f;
    private float verticalVelocity = 0f;
    private bool isJumping = false;

    private Vector2 moveInput;
    private float rotateVelocity;
    private float smoothTime = 0.08f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        if(InputManager.Instance != null)
            InputManager.Instance.StartOrCloseInputMgr(true);
        if(EventCenter.Instance != null)
        {
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, OnHorizontalAxis);
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Vertical, OnVerticalAxis);
        }
        InputManager.Instance.ChangeKeyInfo(E_EventType.E_Jump, KeyCode.Space, InputInfo.E_InputType.Down);
        EventCenter.Instance.AddEventListener(E_EventType.E_Jump, OnReceiveJump);
    }

    void Start()
    {

    }

    
    void Update()
    {
        OnPlayerMoveAndJump();
    }

    private void OnHorizontalAxis(float v)
    {
        moveInput.x = v;
    }

    private void OnVerticalAxis(float v)
    {
        moveInput.y = v;
    }

    private void OnPlayerMoveAndJump()
    {
        float horizontal = moveInput.x;
        float vertical = moveInput.y;
        currentSpeed = data.normalSpeed;
        if (characterController.isGrounded && !isJumping)
        {
            verticalVelocity = -0.5f;
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }    
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        Vector3 gravityDir = new Vector3(0, verticalVelocity, 0);
        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 moveDir = Vector3.zero;
        if (inputDir.magnitude > 0.1f)
        {
            if (!isJumping)
                animator.SetFloat("Speed", inputDir.magnitude);
            Vector3 cameraForward = followingCamera.transform.forward;
            Vector3 cameraRight = followingCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            moveDir = (cameraForward * vertical + cameraRight * horizontal).normalized;
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotateVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
        else
        {
            if (!isJumping)
                animator.SetFloat("Speed", 0f);
        }
        Vector3 finalMoveDir = moveDir * currentSpeed * Time.deltaTime + gravityDir * Time.deltaTime;
        characterController.Move(finalMoveDir);

        if (isJumping)
        {
            if (verticalVelocity < 0 && !characterController.isGrounded)
                animator.CrossFade("JumpFall", 0.5f);
                
            if (verticalVelocity < 0 && characterController.isGrounded)
            {
                animator.CrossFade("JumpLand", 0f);
                StartCoroutine(DelayLandState());
            }
        }
    }

    private void OnReceiveJump()
    {
        if (characterController.isGrounded && !isJumping)
        {
            verticalVelocity = data.jumpForce;
            isJumping = true;
            animator.SetBool("IsJumping", true);
            animator.CrossFade("JumpBegin", 0f);
        }
    }

    IEnumerator DelayLandState()
    {
        yield return null;
        isJumping = false;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsJumping", false);
    }

}
