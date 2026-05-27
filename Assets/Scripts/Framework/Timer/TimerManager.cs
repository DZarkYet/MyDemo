using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器 主要用于开启、停止、重置等操作来管理计时器
/// </summary>
public class TimerManager : BaseManager<TimerManager>
{
    //用于记录当前将要创建的唯一ID的
    private int TIMER_KEY = 0;

    //用于存储管理所有计时器的字典容器
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
    //用于存储管理所有计时器的字典容器(不受Time.timescale影响的计时器)
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    //待移除的列表
    private List<TimerItem> delList = new List<TimerItem>();

    private Coroutine timer;
    private Coroutine realTimer;

    //计时器管理器中的唯一计时用的协同程序 的间隔时间
    private const float intervalTime = 0.1f;

    //为了避免内存的浪费 每次while都会生成 我们之间将其声明为成员变量
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);


    private TimerManager()
    {
        //默认计时器就是开启的
        Start();
    }

    //开启计时器管理器的方法
    public void Start()
    {
        timer = MonoManager.Instance.StartCoroutine(StartTiming(false, timerDic));
        realTimer = MonoManager.Instance.StartCoroutine(StartTiming(true, realTimerDic));
    }

    //关闭计时器管理器的方法
    public void Stop()
    {
        MonoManager.Instance.StopCoroutine(timer);
        MonoManager.Instance.StopCoroutine(realTimer);
    }

    private IEnumerator StartTiming(bool isRealTime, Dictionary<int, TimerItem> timerDic)
    {
        while (true)
        {
            //100毫秒进行一次计时
            if (isRealTime)
                yield return waitForSecondsRealtime;
            else
                yield return waitForSeconds;
            //遍历所有的计时器 进行数据更新
            foreach(TimerItem item in timerDic.Values)
            {
                if (!item.isRunning)
                    continue;
                //判断计时器是否有间隔时间执行的需求
                if(item.callBack != null)
                {
                    //减去100毫秒
                    item.intervalTime -= (int)(intervalTime * 1000);
                    //满足一次间隔时间执行
                    if(item.intervalTime <= 0)
                    {
                        //间隔一定时间 执行一次回调
                        item.callBack.Invoke();
                        //重置间隔时间
                        item.intervalTime = item.maxIntervalTime;
                    }
                }
                //总的时间更新
                item.allTime -= (int)(intervalTime * 1000);
                //计时时间到了 需要执行完成回调函数
                if( item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    delList.Add(item);
                }
            }

            //移除 待移除列表中的数据
            foreach(var item in delList)
            {
                //从字典中移除
                timerDic.Remove(item.keyID);
                //放入缓存池中
                PoolManager.Instance.PushObj<TimerItem>(item);
            }
            //移除结束后清空列表
            delList.Clear();
        }
    }

    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="isRealTime">如果是true 不受time.timescale影响</param>
    /// <param name="allTime">总的时间 毫秒</param>
    /// <param name="overCallBack">总时间结束回调</param>
    /// <param name="intervalTime">间隔计时时间</param>
    /// <param name="callBack">间隔时间结束回调 毫秒</param>
    /// <returns>//返回唯一ID 用于外部控制计时器</returns>
    public int CreateTimer(bool isRealTime, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        //构建唯一ID
        int keyID = ++TIMER_KEY;
        //从缓存池中取出唯一计时器
        TimerItem timerItem = PoolManager.Instance.GetObj<TimerItem>();
        //初始化数据
        timerItem.InitInfo(keyID, allTime, overCallBack, intervalTime, callBack);
        //记录到字典中 进行数据更新
        if(isRealTime)
            realTimerDic.Add(keyID, timerItem);
        else
            timerDic.Add(keyID, timerItem);
        return keyID;
    }

    /// <summary>
    /// 移除单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一IDx</param>
    public void RemoveTimer(int keyID)
    {
        if( timerDic.ContainsKey(keyID))
        {
            //移除对应ID计时器 放入缓存池
            PoolManager.Instance.PushObj<TimerItem>(timerDic[keyID]);
            //从字典中移除
            timerDic.Remove(keyID);
        }
        else if (realTimerDic.ContainsKey(keyID)) 
        {
            //移除对应ID计时器 放入缓存池
            PoolManager.Instance.PushObj<TimerItem>(realTimerDic[keyID]);
            //从字典中移除
            realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void ResetTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }

    /// <summary>
    /// 开启单个计时器 主要用于暂停后重新开启
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StartTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRunning = true;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRunning = true;
        }
    }

    /// <summary>
    /// 停止单个计时器 主要用于暂停
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StopTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRunning = false;
        }
        else if (realTimerDic.ContainsKey(keyID)) 
        {
            realTimerDic[keyID].isRunning = false;
        }
    }

}
