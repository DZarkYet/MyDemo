using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动挂载式的单例模式基类 推荐使用
/// 无需手动挂载 无需动态添加 无需关心切场景带来的问题
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                //动态创建 动态挂载
                GameObject obj = new GameObject();
                //得到T脚本的类名 为对象改名 这样就可以在编辑器中明确看到
                //单例模式脚本对象依附的GameObject
                obj.name = typeof(T).Name;
                //动态挂载对应的 单例模式脚本
                instance = obj.AddComponent<T>();
                //过场景时不移除对象，保证它在整个游戏生命周期中都存在
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // 防止重复实例
        }
    }



}
