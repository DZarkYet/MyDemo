using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器 主要用于切换场景
/// </summary>
public class ScenesManager : BaseManager<ScenesManager>
{
    private ScenesManager() { }

    //同步切换场景的方法
    public void LoadScene(string name, UnityAction callBack = null)
    {
        //切换场景
        SceneManager.LoadScene(name);
        //调用回调
        callBack?.Invoke();
    }

    //异步切换场景的方法
    public void LoadSceneAsync(string name, UnityAction callBack = null)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsync(name, callBack));
    }

    private IEnumerator ReallyLoadSceneAsync(string name, UnityAction callBack = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //不停的在协同程序中每帧检测是否加载结束 如果加载结束就不会进循环逻辑
        while (!ao.isDone)
        {
            //可以在这里利用事件中心 将进度每一帧发送给想要得到的地方
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_Scene_Load, ao.progress);
            yield return null;
        }
        //避免最后一帧结束了 没有同步1出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_Scene_Load, 1);
        callBack?.Invoke();
    }
}
