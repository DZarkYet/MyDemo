using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    public int killCount;
    public int enemyCount;

    private GameManager()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Monster_Dead, AddCount);
        EventCenter.Instance.AddEventListener(E_EventType.E_Game_Start, UpdateGenerate);
    }

    private void UpdateGenerate()
    {
        MonoManager.Instance.AddUpdateListener(GenerateEnemyRandomly);
    }


    public void GenerateEnemyRandomly()
    {
        if(enemyCount <= 6)
        {
            int numX = Random.Range(-20, 21);
            int numZ = Random.Range(-20, 21);
            GameObject enemy = PoolManager.Instance.GetObj("enemy/Enemy");
            enemy.transform.position = new Vector3(numX, 0, numZ);
        }
    }

    private void AddCount()
    {
        killCount++;
    }

}
