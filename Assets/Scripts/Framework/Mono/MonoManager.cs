using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共Mono模块管理器
/// </summary>
public class MonoManager : SingletonAutoMono<MonoManager>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }


    #region 添加和移除事件
    /// <summary>
    /// 添加Update帧更新函数
    /// </summary>
    /// <param name="updateFunc"></param>
    public void AddUpdateListener(UnityAction updateFunc)
    {
        updateEvent += updateFunc;
    }
    /// <summary>
    /// 移除Update帧更新函数
    /// </summary>
    /// <param name="updateFunc"></param>
    public void RemoveUpdateListener(UnityAction updateFunc)
    {
        updateEvent -= updateFunc;
    }
    /// <summary>
    /// 添加FixedUpdate帧更新函数
    /// </summary>
    /// <param name="updateFunc"></param>
    public void AddFixedUpdateListener(UnityAction updateFunc)
    {
        fixedUpdateEvent += updateFunc;
    }
    /// <summary>
    /// 移除FixedUpdate帧更新函数
    /// </summary>
    /// <param name="updateFunc"></param>
    public void RemoveFixedUpdateListener(UnityAction updateFunc)
    {
        fixedUpdateEvent -= updateFunc;
    }
    /// <summary>
    /// 添加LateUpdate帧更新函数
    /// </summary>
    /// <param name="updateFunc"></param>
    public void AddLateUpdateListener(UnityAction updateFunc)
    {
        lateUpdateEvent += updateFunc;
    }
    /// <summary>
    /// 移除LateUpdate帧更新函数
    /// </summary>
    /// <param name="updateFunc"></param>
    public void RemoveLateUpdateListener(UnityAction updateFunc)
    {
        lateUpdateEvent -= updateFunc;
    }
    #endregion

}
