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

    public float hpNow;
    private bool isDead;

    void Awake()
    {
        hpBar.value = 1;
        hpNow = data.maxHP;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = data.speed;
    }
    
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    
    void Update()
    {
        OnHpValueChange();
        LookAtPlayer();
        EnemyState();
        HandleDeadState();
    }

    private void LookAtPlayer()
    {
        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;
        transform.LookAt(playerPos);
    }

    private void EnemyState()
    {
        if (!isDead)
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
        if (animator.GetBool("Attack"))
            return;
        animator.SetBool("Attack", true);
        MusicManager.Instance.PlaySound("monster", false);

        if (attackHitboxCoroutine != null)
            StopCoroutine(attackHitboxCoroutine);
        attackHitboxCoroutine = StartCoroutine(SpawnAttackHitbox());
    }

    private void HandleDeadState()
    {
        if (!isDead) return;
        animator.CrossFade("Dead", 0f);
        PoolManager.Instance.PushObj(this.gameObject);
        isDead = false;
    }

    private void OnHpValueChange()
    {
        if(hpNow <= 0)
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
        animator.SetBool("Hit", true);
        hpNow -= damage;
        StartCoroutine(ResetHitBool());
    }
    IEnumerator ResetHitBool()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("Hit", false);
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
    }

}
