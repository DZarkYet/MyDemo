using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举
/// </summary>
public enum E_EventType
{
    /// <summary>
    /// 怪物死亡事件 -- 参数:Monster
    /// </summary>
    E_Mnster_Dead,
    /// <summary>
    /// 玩家获取奖励 -- 参数:int
    /// </summary>
    E_Player_GetReward,
    /// <summary>
    /// 测试用事件 -- 参数:无
    /// </summary>
    E_Test,
    /// <summary>
    /// 场景变换得到进度 -- 参数:无
    /// </summary>
    E_Scene_Load,

    /// <summary>
    /// 输入系统触发技能1的行为
    /// </summary>
    E_Input_Skill1,
    E_Input_Skill2,
    E_Input_Skill3,
    
    /// <summary>
    /// 水平热键 -1~1的监听
    /// </summary>
    E_Input_Horizontal,
    /// <summary>
    /// 竖直热键 -1~1的监听
    /// </summary>
    E_Input_Vertical,
    /// <summary>
    /// 监听跳跃 -- 参数:无
    /// </summary>
    E_Jump,
    /// <summary>
    /// 监听攻击 -- 参数:无
    /// </summary>
    E_Player_Attack,
    /// <summary>
    /// 监听闪避/翻滚 -- 参数:无
    /// </summary>
    E_Player_Sprint,
    /// <summary>
    /// 关闭注册面板
    /// </summary>
    E_Close_Registry,
}
