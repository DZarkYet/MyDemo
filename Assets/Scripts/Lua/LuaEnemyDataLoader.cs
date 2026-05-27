using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaEnemyDataLoader : MonoBehaviour
{
    public EnemyDataSO enemyData;

    void Start()
    {
        if (LuaManager.Instance == null || enemyData == null) return;

        LuaTable table = LuaManager.Instance.DoFile("config/enemy_data");
        if (table == null) return;

        enemyData.characterName = "Enemy";
        enemyData.speed = table.Get<float>("speed");
        enemyData.maxHP = table.Get<float>("maxHP");
        enemyData.attackPower = table.Get<float>("attackPower");
        enemyData.detectRange = table.Get<float>("detectRange");
        enemyData.attackRange = table.Get<float>("attackRange");
        enemyData.chaseRange = table.Get<float>("chaseRange");
        enemyData.patrolRadius = table.Get<float>("patrolRadius");

        Debug.Log("Enemy数据热更新完成");
        table.Dispose();
    }


}
