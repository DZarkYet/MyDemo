using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboDataSO", menuName = "ScriptableObject/ComboData", order = 2)]
public class ComboDataSO : ScriptableObject
{
    public ComboInfo[] combos;
}

[System.Serializable]
public class ComboInfo
{
    public Vector3 hitboxCenter;    // 相对角色的偏移
    public Vector3 hitboxSize;      // 判定框大小
    public float activeDuration;    // 持续帧
    public int damage;
}
