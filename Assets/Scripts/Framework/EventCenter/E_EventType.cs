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
    E_Monster_Dead,
    /// <summary>
    /// 场景变换得到进度 -- 参数:无
    /// </summary>
    E_Scene_Load,
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
    /// <summary>
    /// 场景加载结束
    /// </summary>
    E_End_Loading,
    /// <summary>
    /// 锁定鼠标
    /// </summary>
    E_Mouse_Lock,
    /// <summary>
    /// 解除鼠标锁定
    /// </summary>
    E_Mouse_UnLock,
    /// <summary>
    /// 计时结束
    /// </summary>
    E_Times_Up,
    /// <summary>
    /// 开始游戏
    /// </summary>
    E_Game_Start,
    /// <summary>
    /// 玩家受伤
    /// </summary>
    E_Player_Hit,
}
