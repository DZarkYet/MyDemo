using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于进行加载AB相关资源的整合 在开发中可以通过EditorResManager去加载对应资源进行测试
/// </summary>
public class ABResManager : BaseManager<ABResManager>
{
    //如果是true会通过EditorResManager加载 false则通过ABManager加载
    private bool isDebug = false;

    private ABResManager() { }

    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false, bool isMp3 = true) where T : Object
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            //我们自定义了一个AB包中资源管理的方式 对应文件夹名 就是包名
            T res = EditorResManager.Instance.LoadEditorRes<T>($"{abName}/{resName}", isMp3);
            callBack?.Invoke(res);
        }
        else
        {
            ABManager.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
        }
#else
    ABManager.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
    }
}
