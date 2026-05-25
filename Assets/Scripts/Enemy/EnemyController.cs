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
    [Header("×é¼þÉèÖÃ")]
    public Slider hpBar;
    public EnemyDataSO data;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
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
        if (isDead)
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
                //¾²Ö¹×´Ì¬
                case E_Enemy_State.E_Idle:
                    animator.SetFloat("Speed", 0f);
                    animator.SetBool("Attack", false);
                    agent.isStopped = true;
                    if(distance <= data.detectRange && distance > data.attackRange)
                        currentState = E_Enemy_State.E_Run;
                    break;
                //×·×Ù×´Ì¬
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
    }

    private void HandleDeadState()
    {
        animator.CrossFade("Dead", 0f);
        PoolManager.Instance.PushObj(this.gameObject);
    }

    private void OnHpValueChange()
    {
        if(hpNow <= 0)
        {
            hpNow = 0;
            isDead = true;
        }
        hpBar.value = hpNow / data.maxHP;
    }

    public void TakeDamage(float damage)
    {
        animator.SetBool("Hit", true);
        hpNow -= damage;
    }
}
