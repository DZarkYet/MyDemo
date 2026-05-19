using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器对象 里面存储了计时器的相关数据
/// </summary>
public class TimerItem : IPoolObject
{
    //唯一ID
    public int keyID;
    //计时结束后的委托回调
    public UnityAction overCallBack;
    //间隔一定时间的委托回调
    public UnityAction callBack;
    //毫秒 表示计时器总的计时时间 毫秒 1s = 1000ms
    public int allTime;
    //记录 一开始计时时的总时间 用于时间重置
    public int maxAllTime;
    //间隔执行回调的时间
    public int intervalTime;
    //记录 一开始的间隔时间 毫秒 1s = 1000ms
    public int maxIntervalTime;
    //是否在进行计时
    public bool isRunning;

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="allTime">总的时间</param>
    /// <param name="overCallBack">总时间计时结束后的回调</param>
    /// <param name="intervalTime">间隔时间</param>
    /// <param name="callBack">间隔执行时间结束后的回调</param>
    public void InitInfo(int keyID, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRunning = true;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRunning = true;
    }

    /// <summary>
    /// 缓存池回收时 清除相关引用
    /// </summary>
    public void ResetInfo()
    {
        this.overCallBack = null;
        this.callBack = null;
    }


}
