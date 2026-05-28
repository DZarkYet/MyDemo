using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonAutoMono<GameManager>
{
    public int killCount = 0;
    private int enemyCount;
    private GameObject[] enemys;
    private bool isStart;
    public bool isGameStarted => isStart;

    private GameManager() { }

    protected override void Awake()
    {
        base.Awake();
        base.Awake();
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_Dead, AddCount);
        Debug.Log("GameManager Awake, 对象ID = " + GetInstanceID());
        EventCenter.Instance.AddEventListener(E_EventType.E_Times_Up, () =>
        {
            RemoveAllEnemy();
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Game_Start, () =>
        {
            isStart = true;
            killCount = 0;
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Player_Dead, () =>
        {
            RemoveAllEnemy();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Pause, () =>
        {
            isStart = false;
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Resume, () =>
        {
            isStart = true;
        });
    }

    private void Update()
    {
        if(isStart)
            GenerateEnemyRandomly();
    }


    private void GenerateEnemyRandomly()
    {
        if(enemyCount <= 6)
        {
            int numX = Random.Range(-20, 21);
            int numZ = Random.Range(-20, 21);
            GameObject enemy = PoolManager.Instance.GetObj("enemy/Enemy");
            enemy.transform.position = new Vector3(numX, 0, numZ);
            enemyCount++;
        }
    }

    private void RemoveAllEnemy()
    {
        isStart = false;
        enemyCount = 0;
        enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var item in enemys)
        {
            PoolManager.Instance.PushObj(item);
        }
        enemys = null;
    }

    private void AddCount()
    {
        Debug.Log("AddCount 被调用！killCount=" + killCount);
        killCount++;
        enemyCount--;
    }

}
