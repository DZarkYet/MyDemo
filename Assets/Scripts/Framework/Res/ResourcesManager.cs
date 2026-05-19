using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源信息基类 主要用于里氏替换原则
/// </summary>
public abstract class ResBaseInfo
{
    //引用计数
    public int refCount;
}

/// <summary>
/// 资源信息对象 主要用于存储资源信息 异步加载委托信息 异步加载协程信息
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : ResBaseInfo
{
    //资源
    public T asset;
    //主要用于异步加载结束后 传递资源到外部的委托
    public UnityAction<T> callBack;
    //用于存储异步加载时 开启的协程程序
    public Coroutine coroutine;
    //决定引用计数为0时 是否真正需要移除
    public bool isDel = false;

    public void AddRefCount()
    {
        refCount++;
    }
    public void SubRefCount()
    {
        refCount--;
        if(refCount < 0)
        {
            Debug.LogError("引用计数小于0，请检查引用与卸载是否配队");
        }
    }
}

/// <summary>
/// Resources资源加载模块管理器
/// </summary>
public class ResourcesManager : BaseManager<ResourcesManager>
{
    //用于存储加载过的资源 或者加载中的资源的容器
    private Dictionary<string, ResBaseInfo> resDic = new Dictionary<string, ResBaseInfo>();

    private ResourcesManager() { }

    /// <summary>
    /// 同步价值资源Resources的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo = new ResInfo<T>();
        //字典中不存在资源时
        if (!resDic.ContainsKey(resName))
        {
            //直接同步加载 并且记录资源信息 到字典中 方便下次取出来用
            T res = Resources.Load<T>(path);
            resInfo = new ResInfo<T>();
            resInfo.asset = res;
            //引用计数增加
            resInfo.AddRefCount();
            resDic.Add(resName, resInfo);
            return res;
        }
        else
        {
            resInfo = resDic[resName] as ResInfo<T>;
            //引用计数增加
            resInfo.AddRefCount();
            //存在异步加载还在加载中
            if(resInfo.asset == null)
            {
                //停止异步加载   
                MonoManager.Instance.StopCoroutine(resInfo.coroutine);
                //直接采用同步方式加载成功
                T res = Resources.Load<T>(path);
                //记录
                resInfo.asset = res;
                //还应该把那些等待着异步加载的委托去执行
                resInfo.callBack?.Invoke(res);
                //回调结束 异步加载停止 清除无用引用
                resInfo.callBack = null;
                resInfo.coroutine = null;
                //并使用
                return res;
            }
            else
            {
                //如果已经加载结束 直接用
                return resInfo.asset;
            }
        }

    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callBack">加载结束后的回调函数 当异步加载结束后才会调用</param>
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : UnityEngine.Object
    {
        //资源的唯一id: 路径名_资源类型
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> resInfo;
        if (!resDic.ContainsKey(resName))
        {
            //声明一个资源信息对象
            resInfo = new ResInfo<T>();
            //引用计数增加
            resInfo.AddRefCount();
            //将资源记录添加到字典中(资源还没加载成功)
            resDic.Add(resName, resInfo);
            //记录传入的委托函数 一会儿加载完成了再使用
            resInfo.callBack += callBack;
            //开启协程去进行 异步加载 并记录协同程序 用于之后可能的停止
            resInfo.coroutine = MonoManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            //从字典中取出资源信息
            resInfo = resDic[resName] as ResInfo<T>;
            //引用计数增加
            resInfo.AddRefCount();
            //如果资源还没有加载完成
            //意味着还在进行异步加载
            if (resInfo.asset == null)
                resInfo.callBack += callBack;
            else
                callBack?.Invoke(resInfo.asset);
        }

        //要通过协同程序去异步加载资源
        //MonoManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path, callBack));
    }

    private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        //等待资源加载结束后 才会继续执行 yield return 后面的内容
        yield return rq;

        string resName = path + "_" + typeof(T).Name;
        //资源加载结束 将资源传到外部的委托函数中去进行使用
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //取出资源信息 记录加载完成的资源
            resInfo.asset = rq.asset as T;
            //如果发现需要删除 再去移除资源
            //引用计数为0 才真正去移除
            if (resInfo.refCount == 0)
            {
                UnloadAsset<T>(path, resInfo.isDel, null, false);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后这些引用就可以清空了 避免引用的占用可能带来的内存泄露问题
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }

    [Obsolete("注意：建议使用泛型加载方式，如果一定要使用这个方式，一定不要同泛型方式混合使用")]
    public void LoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callBack)
    {
        //资源的唯一id: 路径名_资源类型
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> resInfo;
        if (!resDic.ContainsKey(resName))
        {
            //声明一个资源信息对象
            resInfo = new ResInfo<UnityEngine.Object>();
            //引用计数增加
            resInfo.AddRefCount();
            //将资源记录添加到字典中(资源还没加载成功)
            resDic.Add(resName, resInfo);
            //记录传入的委托函数 一会儿加载完成了再使用
            resInfo.callBack += callBack;
            //开启协程去进行 异步加载 并记录协同程序 用于之后可能的停止
            resInfo.coroutine = MonoManager.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            //从字典中取出资源信息
            resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数增加
            resInfo.AddRefCount();
            //如果资源还没有加载完成
            //意味着还在进行异步加载
            if (resInfo.asset == null)
                resInfo.callBack += callBack;
            else
                callBack?.Invoke(resInfo.asset);
        }
    }

    private IEnumerator ReallyLoadAsync(string path, Type type)
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync(path, type);
        //等待资源加载结束后 才会继续执行 yield return 后面的内容
        yield return rq;

        string resName = path + "_" + type.Name;
        //资源加载结束 将资源传到外部的委托函数中去进行使用
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //取出资源信息 记录加载完成的资源
            resInfo.asset = rq.asset;
            //引用计数为0 才真正去移除
            if (resInfo.refCount == 0)
            {
                UnloadAsset(path, type, resInfo.isDel, null, false);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后这些引用就可以清空了 避免引用的占用可能带来的内存泄露问题
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <param name="assetToUnload"></param>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + typeof(T).Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //引用计数减一
            if(isSub)
                resInfo.SubRefCount();
            //记录引用计数为0时 是否马上移除的标签
            resInfo.isDel = isDel;
            //资源已经加载结束
            if(resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel == true)
            {
                //从字典移除
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            //资源正在异步加载中
            else if(resInfo.asset == null)
            {
                //MonoManager.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);
                //为了保险起见 一定要让资源移除了
                //改变标识 待删除
                //resInfo.isDel = true;

                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    public void UnloadAsset(string path, Type type, bool isDel = false, UnityAction<UnityEngine.Object> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + typeof(UnityEngine.Object).Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数减一
            if(isSub)
                resInfo.SubRefCount();
            //记录引用计数为0时 是否马上移除的标签
            resInfo.isDel = isDel;
            //资源已经加载结束
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel == true)
            {
                //从字典移除
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            //资源正在异步加载中
            else if (resInfo.asset == null)
            {
                //MonoManager.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);
                //为了保险起见 一定要让资源移除了
                //改变标识 待删除
                //resInfo.isDel = true;
                //当异步加载不想使用时 我们应该移除其回调 而不是直接去卸载资源
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    /// <summary>
    /// 异步卸载对应没有使用的Resources资源
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public void UnloadUnusedAssets(UnityAction callBack)
    {
        MonoManager.Instance.StartCoroutine(ReallyUnloadAssets(callBack));
    }

    private IEnumerator ReallyUnloadAssets(UnityAction callBack)
    {
        //就是在真正移除不使用的资源之前 应该把我们自己记录的那些引用计数为0 并且没有被移除的资源移除
        List<string> list = new List<string>();
        foreach(string resName in resDic.Keys)
        {
            if (resDic[resName].refCount == 0)
            {
                list.Add(resName);
            }
        }
        foreach(string resName in list)
        {
            resDic.Remove(resName);
        }

        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }

    /// <summary>
    /// 获取当前某个资源的引用计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetRefCount<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            return (resDic[resName] as ResInfo<T>).refCount;
        }
        return 0;
    }

    /// <summary>
    /// 清空字典
    /// </summary>
    /// <param name="callBack"></param>
    public void ClearDic(UnityAction callBack)
    {
        MonoManager.Instance.StartCoroutine(ReallyClearDic(callBack));
    }

    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }
}
