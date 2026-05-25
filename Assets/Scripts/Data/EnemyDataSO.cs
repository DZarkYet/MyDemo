using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ScriptableObject/EnemyData", order = 0)]
public class EnemyDataSO : ScriptableObject
{
    [Header("基础设置")]
    public string characterName = "Enemy";
    public float speed = 4f;
    public float maxHP = 80f;
    public float attackPower = 15f;

    [Header("AI行为参数")]
    public float detectRange = 10f;
    public float attackRange = 2f;
    public float chaseRange = 15f;
    public float patrolRadius = 5f;
}
