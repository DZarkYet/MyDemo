using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("组件设置")]
    private CharacterController characterController;
    private Animator animator;
    private Transform followingCamera;
    public PlayerDataSO data;
    public ComboDataSO comboData;
    public GameObject sword;
    public GameObject packedSword;
    private Coroutine delayComboCoroutine;
    private Coroutine delaySwordCoroutine;
    private Coroutine delaySprintCoroutine;
    private Coroutine delaySwordActiveCoroutine;
    private Coroutine disableHitboxCoroutine;
    private GameObject currentHitbox;

    [Header("基础设置")]
    public float currentSpeed;
    public float gravity = 9.81f;
    private float verticalVelocity = 0f;
    private bool isJumping = false;
    private int comboCount = 0;
    private bool isSprinting = false;

    private GameObject[] attackTarget;
    private Vector2 moveInput;
    private float rotateVelocity;
    private float smoothTime = 0.08f;

    private void Awake()
    {
        //获取角色控制器组件和动画组件的引用，以便在后续的移动和动画逻辑中使用
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        followingCamera = Camera.main.transform;
        //启动输入管理器，并注册水平轴、垂直轴的监听器，以便在玩家输入时能够正确处理移动逻辑
        if (InputManager.Instance != null)
            InputManager.Instance.StartOrCloseInputMgr(true);
        if(EventCenter.Instance != null)
        {
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, OnHorizontalAxis);
            EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Vertical, OnVerticalAxis);
        }
        //注册跳跃输入事件，监听空格键按下事件，并在触发时调用OnReceiveJump方法处理跳跃逻辑
        InputManager.Instance.ChangeKeyInfo(E_EventType.E_Jump, KeyCode.Space, InputInfo.E_InputType.Down);
        EventCenter.Instance.AddEventListener(E_EventType.E_Jump, OnReceiveJump);
        //注册攻击输入事件，监听鼠标左键按下事件，并在触发时调用OnReceiveAttack方法处理攻击逻辑
        InputManager.Instance.ChangeMouseInfo(E_EventType.E_Player_Attack, 0, InputInfo.E_InputType.Down);
        EventCenter.Instance.AddEventListener(E_EventType.E_Player_Attack, OnReceiveAttack);
        InputManager.Instance.ChangeMouseInfo(E_EventType.E_Player_Sprint, 1, InputInfo.E_InputType.Down);
        EventCenter.Instance.AddEventListener(E_EventType.E_Player_Sprint, OnReceiveSprint);

        MusicManager.Instance.ChangeSoundValue(0.5f);

        sword.SetActive(false);
        packedSword.SetActive(true);
    }

    void Start()
    {

    }

    
    void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            OnPlayerMoveAndJump();
        }     
        OnCancelSprint();
    }

    #region 水平移动和跳跃逻辑

    /// <summary>
    /// 接受水平轴输入，更新moveInput.x的值
    /// </summary>
    /// <param name="v"></param>
    private void OnHorizontalAxis(float v)
    {
        moveInput.x = v;
    }

    /// <summary>
    /// 接受垂直轴输入，更新moveInput.y的值
    /// </summary>
    /// <param name="v"></param>
    private void OnVerticalAxis(float v)
    {
        moveInput.y = v;
    }

    /// <summary>
    /// 处理玩家的移动和跳跃逻辑，包括根据输入计算移动方向、应用重力、更新动画状态等
    /// </summary>
    private void OnPlayerMoveAndJump()
    {
        float horizontal = moveInput.x;
        float vertical = moveInput.y;
        if (!isSprinting)
        {
            currentSpeed = data.normalSpeed;
        }
        //如果玩家在地面上且当前不处于跳跃状态，则将垂直速度设置为一个小的负值以保持角色贴地，并确保跳跃状态为false
        if (characterController.isGrounded && !isJumping)
        {
            verticalVelocity = -0.5f;
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }
        //否则，如果玩家在空中，则应用重力影响垂直速度
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        Vector3 gravityDir = new Vector3(0, verticalVelocity, 0);
        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 moveDir = Vector3.zero;
        //如果输入方向的大小大于0.1（即玩家有明显的输入），则根据摄像机的朝向计算移动方向，并平滑旋转角色朝向目标角度
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
        //如果玩家处于跳跃状态，根据垂直速度和是否接触地面来更新动画状态，分别切换到跳跃下落或落地动画，并在落地后通过协程延迟重置跳跃状态
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

    /// <summary>
    /// 接受跳跃输入事件
    /// </summary>
    private void OnReceiveJump()
    {
        //如果玩家在地面上且当前不处于跳跃状态，则设置垂直速度为跳跃力，并更新动画状态为跳跃开始
        if (characterController.isGrounded && !isJumping)
        {
            verticalVelocity = data.jumpForce;
            isJumping = true;
            animator.SetBool("IsJumping", true);
            animator.CrossFade("JumpBegin", 0f);
        }
    }

    /// <summary>
    /// 延迟处理落地状态的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayLandState()
    {
        //首先等待一帧以确保动画状态正确更新
        yield return null;
        isJumping = false;
        //然后设置isJumping为false，并在0.1秒后将动画状态中的IsJumping参数重置为false，以结束跳跃动画
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsJumping", false);
    }
    #endregion

    //接受攻击信息并处理攻击逻辑
    private void OnReceiveAttack()
    {
        LookAtNearestEnemy();
        if (delaySwordActiveCoroutine != null)
            StopCoroutine(delaySwordActiveCoroutine);
        packedSword.SetActive(false);
        sword.SetActive(true);
        comboCount++;
        if(delayComboCoroutine != null)
            StopCoroutine(delayComboCoroutine);
        if(delaySwordCoroutine != null)
            StopCoroutine(delaySwordCoroutine);
        if (comboCount > 5)
            comboCount = 1;
        switch (comboCount)
        {
            case 1:
                animator.CrossFade("Combo1", 0f);
                SpawnAttackHitbox(comboCount);
                MusicManager.Instance.PlaySound("attack1");
                MusicManager.Instance.PlaySound("normalSwoosh", false);
                break;
            case 2:
                animator.CrossFade("Combo2", 0f);
                SpawnAttackHitbox(comboCount);
                MusicManager.Instance.PlaySound("normalSwoosh", false);
                break;
            case 3:
                animator.CrossFade("Combo3", 0f);
                SpawnAttackHitbox(comboCount);
                MusicManager.Instance.PlaySound("attack2");
                MusicManager.Instance.PlaySound("windSwoosh", false);
                break;
            case 4:
                animator.CrossFade("Combo4", 0f);
                SpawnAttackHitbox(comboCount);
                MusicManager.Instance.PlaySound("windSwoosh", false);
                break;
            case 5:
                animator.CrossFade("Combo5", 0f);
                SpawnAttackHitbox(comboCount);
                MusicManager.Instance.PlaySound("attack3");
                MusicManager.Instance.PlaySound("lastSwoosh");
                delaySwordActiveCoroutine = StartCoroutine(DelayCombo5Effect());
                break;
        }
        delayComboCoroutine = StartCoroutine(DelayComboWindow());
        delaySwordCoroutine = StartCoroutine(DelaySwordState());
    }

    private void SpawnAttackHitbox(int comboIndex)
    {
        if (disableHitboxCoroutine != null)
            StopCoroutine(disableHitboxCoroutine);
        if (currentHitbox != null)
        {
            currentHitbox.SetActive(false);
            PoolManager.Instance.PushObj(currentHitbox);
            currentHitbox = null;
        }

        ComboInfo data = comboData.combos[comboIndex - 1];

        currentHitbox = PoolManager.Instance.GetObj("AttackHitbox");
        currentHitbox.transform.SetParent(this.transform);
        currentHitbox.transform.localPosition = data.hitboxCenter;
        currentHitbox.transform.localRotation = Quaternion.identity;
        currentHitbox.transform.localScale = data.hitboxSize;

        // 伤害写到判定框的碰撞脚本上
        var hitboxScript = currentHitbox.GetComponent<AttackHitbox>();
        if (hitboxScript != null)
            hitboxScript.damage = data.damage;

        disableHitboxCoroutine = StartCoroutine(DisableHitbox(data.activeDuration));
    }

    private IEnumerator DisableHitbox(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentHitbox != null)
        {
            currentHitbox.SetActive(false);
            PoolManager.Instance.PushObj(currentHitbox);
            currentHitbox = null;
        }
    }


    //延迟清空连段
    IEnumerator DelayComboWindow()
    {
        yield return new WaitForSeconds(2);
        comboCount = 0;
    }

    //延迟清空背剑与手持剑状态
    IEnumerator DelaySwordState()
    {
        yield return new WaitForSeconds(2);
        sword.SetActive(false);
        packedSword.SetActive(true);
    }

    //接收冲刺的消息
    private void OnReceiveSprint()
    {
        if(moveInput.magnitude > 0.1)
        {
            isSprinting = true;
            animator.CrossFade("Sprint", 0f);
            animator.SetBool("IsSprinting", true);
            currentSpeed = data.sprintSpeed;
            delaySprintCoroutine = StartCoroutine(DelaySprintState());
        }
    }

    //延迟处理冲刺的停止逻辑，设置冲刺时长
    IEnumerator DelaySprintState()
    {
        yield return new WaitForSeconds(2f);
        isSprinting = false;
        animator.SetBool("IsSprinting", false);
    }

    //取消冲刺逻辑
    private void OnCancelSprint()
    {
        if(moveInput.magnitude < 0.1f)
        {
            animator.SetBool("IsSprinting", false);
            if (delaySprintCoroutine != null)
                StopCoroutine(delaySprintCoroutine);
        }
    }

    //延迟播放第五段攻击的特效动画，使其匹配攻击动画
    IEnumerator DelayCombo5Effect()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject effectObj = PoolManager.Instance.GetObj("effect/Electro slash");
        effectObj.transform.position = this.transform.position + this.transform.forward * 1f;
        effectObj.transform.rotation = this.transform.rotation;
    }

    private void LookAtNearestEnemy()
    {
        attackTarget = GameObject.FindGameObjectsWithTag("Enemy");
        if (attackTarget.Length > 0)
        {
            GameObject nearestEnemy = null;
            float minDistance = 10000f;
            foreach (GameObject enemy in attackTarget)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    nearestEnemy = enemy;
                    minDistance = distance;
                }
            }
            Vector3 enemyPos = nearestEnemy.transform.position;
            enemyPos.y = transform.position.y;
            transform.LookAt(enemyPos);
        }
    }
}
