using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


enum E_Enemy_State
{
    E_Idle,
    E_Run,
    E_Attack
}


public class EnemyController : MonoBehaviour
{
    [Header("组件设置")]
    public Slider hpBar;
    public EnemyDataSO data;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private GameObject currentAttackHitbox;
    private Coroutine attackHitboxCoroutine;

    private E_Enemy_State currentState = E_Enemy_State.E_Idle;

    [Header("基础设置")]
    public float hpNow;
    private bool isHit = false;
    private bool isDead;
    private bool isDying;
    private float attackCooldown = 0f;
    private bool isStart = true;

    void Awake()
    {
        hpBar.value = 1;
        hpNow = data.maxHP;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = data.speed;
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Pause, OnPause);
        EventCenter.Instance.AddEventListener(E_EventType.E_Resume, OnResume);
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isStart)
        {
            OnHpValueChange();
            LookAtPlayer();
            if (attackCooldown > 0f)
                attackCooldown -= Time.deltaTime;
            if (!isHit)
                EnemyState();
            HandleDeadState();
        }
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Pause, OnPause);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Resume, OnResume);
    }

    private void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;
        transform.LookAt(playerPos);
    }

    private void EnemyState()
    {
        if (!isDead && player != null)
        {
            float distance = Vector3.Distance(player.position, this.transform.position);
            switch (currentState)
            {
                //静止状态
                case E_Enemy_State.E_Idle:
                    animator.SetFloat("Speed", 0f);
                    animator.SetBool("Attack", false);
                    agent.isStopped = true;
                    if(distance <= data.detectRange && distance > data.attackRange)
                        currentState = E_Enemy_State.E_Run;
                    break;
                //追踪状态
                case E_Enemy_State.E_Run:
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    animator.SetFloat("Speed", 1f);
                    animator.SetBool("Attack", false);
                    if(distance > data.chaseRange)
                        currentState = E_Enemy_State.E_Idle;
                    else if(distance <= data.attackRange)
                        currentState = E_Enemy_State.E_Attack;
                    break;
                case E_Enemy_State.E_Attack:
                    agent.isStopped = true;
                    HandleAttack();
                    if (distance > data.attackRange)
                        currentState = E_Enemy_State.E_Run;
                    else
                        animator.SetFloat("Speed", 0f);
                    break;
            }
        }
    }

    private void HandleAttack()
    {
        if (animator.GetBool("Attack") || attackCooldown > 0) return;
        animator.SetBool("Attack", true);
        MusicManager.Instance.PlaySound("monster", false);

        if (attackHitboxCoroutine != null)
            StopCoroutine(attackHitboxCoroutine);
        attackHitboxCoroutine = StartCoroutine(SpawnAttackHitbox());
    }

    private void HandleDeadState()
    {
        if (!isDead || isDying) return;
        isDying = true;
        animator.CrossFade("Dead", 0f);
        StartCoroutine(DelayPushEnemy());
    }

    IEnumerator DelayPushEnemy()
    {
        isDying = false;
        yield return new WaitForSeconds(1f);
        isDead = false;
        PoolManager.Instance.PushObj(this.gameObject);
        hpNow = data.maxHP;
        animator.Rebind();
    }

    private void OnHpValueChange()
    {
        if (isDead) return;
        if (hpNow <= 0)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Monster_Dead);
            hpNow = 0;
            isDead = true;
        }
        hpBar.value = hpNow / data.maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        animator.CrossFade("Hit", 0f);
        isHit = true;
        hpNow -= damage;
        StartCoroutine(ResetHitBool());
    }
    IEnumerator ResetHitBool()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("Hit", false);
        isHit = false;
    }

    // 攻击动画结束事件 → 重置 Attack bool
    public void OnAttackEnd()
    {
        animator.SetBool("Attack", false);
    }

    private IEnumerator SpawnAttackHitbox()
    {
        yield return new WaitForSeconds(0.15f);
        currentAttackHitbox = PoolManager.Instance.GetObj("EnemyAttackHitbox");
        currentAttackHitbox.SetActive(false);
        currentAttackHitbox.transform.SetParent(transform);
        // 固定值：前方1米，1米宽高
        currentAttackHitbox.transform.localPosition = new Vector3(0, 1f, 1f);
        currentAttackHitbox.transform.localRotation = Quaternion.identity;
        currentAttackHitbox.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        var hitbox = currentAttackHitbox.GetComponent<EnemyAttackHitbox>();
        if (hitbox != null)
            hitbox.damage = data.attackPower;   // 直接用 SO 里的攻击力

        currentAttackHitbox.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        if (currentAttackHitbox != null)
        {
            currentAttackHitbox.SetActive(false);
            PoolManager.Instance.PushObj(currentAttackHitbox);
            currentAttackHitbox = null;
        }
        animator.SetBool("Attack", false);
        attackCooldown = 2f;
    }

    private void OnPause()
    {
        isStart = false;
    }

    private void OnResume()
    {
        isStart = true;
    }

}
