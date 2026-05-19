using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 层级枚举
/// </summary>
public enum E_UILayer
{
    //最底层
    Bottom,
    //中层
    Middle,
    //高层
    Top,
    //系统层（最高层）
    System
}

/// <summary>
/// 管理所有UI面板的管理器
/// 面板预制体名要和面板类名一致
/// </summary>
public class UIManager : BaseManager<UIManager>
{
    /// <summary>
    /// 用于
    /// </summary>
    private abstract class BasePanelInfo { }

    /// <summary>
    /// 用于存储面板信息 和加载完成后的回调函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class PanelInfo<T> : BasePanelInfo where T : BasePanel
    {
        public T panel;
        public UnityAction<T> callBack;
        public bool isHide;

        public PanelInfo(UnityAction<T> callBack)
        {
            this.callBack += callBack;
        }
    }

    private Camera uiCamera;
    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    //层级父对象
    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    //用于存储所有的面板对象
    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

    private UIManager()
    {
        //动态创建唯一的Canvas和EventSystem（摄像机）
        uiCamera = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        //UI摄像机过场景不移除 专门用来渲染UI面板
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);

        //动态创建Canvas
        uiCanvas = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        //使用的UI摄像机
        uiCanvas.worldCamera = uiCamera;
        //过场景不移除
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        //找到层级父对象
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");

        //动态创建EventSystem
        uiEventSystem = GameObject.Instantiate(ResourcesManager.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    /// <summary>
    /// 获取对应层级的对象
    /// </summary>
    /// <param name="layer">层级枚举值</param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;  
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    /// <param name="panelName">面板的名字</param>
    /// <param name="layer">面板显示的层级</param>
    /// <param name="callBack">由于可能是异步加载 因此通过委托回调的形式 将加载完成的面板传递出去使用</param>
    /// <param name="isSync">是否采用同步加载 默认为false</param>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null, bool isSync = false) where T : BasePanel
    {
        //获取面板名 面板预制体名必须和面板类名
        string panelName = typeof(T).Name;
        //存在面板
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在异步加载中
            if (panelInfo.panel == null)
            {
                //如果之前显示了又隐藏 现在又想显示 直接设为false
                panelInfo.isHide = false;

                //如果正在异步加载 应该等待加载完毕 只需要记录回调函数 加载完调用即可
                if(callBack != null)
                    panelInfo.callBack += callBack;
            }
            else//已经加载结束
            {
                //如果是失活状态 直接激活面板就可以显示
                if (!panelInfo.panel.gameObject.activeSelf)
                    panelInfo.panel.gameObject.SetActive(true);

                //如果要显示面板 会执行一次面板的默认显示逻辑
                panelInfo.panel.ShowMe();
                //如果存在回调 直接返回出去即可
                callBack?.Invoke(panelInfo.panel);
            }
            return;
        }
        //不存在面板 先存入字典当中占个位置 之后如果又显示 我才能得到字典中的信息进行判读
        panelDic.Add(panelName, new PanelInfo<T>(callBack));

        //不存在面板 加载面板
        ABResManager.Instance.LoadResAsync<GameObject>("ui", panelName, (res) =>
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //表示异步加载结束前就想要隐藏该面板
            if (panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }

            //层级的处理
            Transform father = GetLayerFather(layer);
            //避免没有按照指定规则传递层级参数 避免为空
            if (father == null)
                father = middleLayer;
            //将面板预制体创建到对应父对象下 并保持原本缩放大小
            GameObject panelObj = GameObject.Instantiate(res, father, false);
 
            //获取对应UI组件返回出去
            T panel = panelObj.GetComponent<T>();
            //显示面板时执行的默认方法
            panel.ShowMe();
            //传出去使用
            panelInfo.callBack?.Invoke(panel);
            //回调执行完 将其清空 避免内存泄露
            panelInfo.callBack = null;
            //存储panel
            panelInfo.panel = panel;
            
        }, isSync);
        
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //如果存在但是正在加载中
            if(panelInfo.panel == null)
            {
                //修改隐藏标识 表示这个面板即将隐藏
                panelInfo.isHide = true;
                //既然要隐藏了 回调函数都不会调用 直接置空
                panelInfo.callBack = null;
            }
            else//已经加载结束
            {
                //执行默认隐藏面板的逻辑
                panelInfo.panel.HideMe();
                //如果要销毁 就直接将面板销毁 并从字典记录里移除
                if (isDestroy)
                {
                    //销毁面板
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    //从容器中移除
                    panelDic.Remove(panelName);
                }
                //如果不销毁 只是失活 下次再显示时复用
                else
                    panelInfo.panel.gameObject.SetActive(false);
            }
        }

    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if(panelInfo.panel == null)
            {
                //加载中 应该等待加载结束 再通过回调传递给外部使用
                panelInfo.callBack += callBack;
            }
            else if(!panelInfo.isHide)//加载结束 并且没有隐藏
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }        
    }

    /// <summary>
    /// 为控件添加自定义事件
    /// </summary>
    /// <param name="control">对应的控件</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">响应的函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        //这种逻辑主要是用于保证 控件上只挂载一个EventTrigger组件
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if(trigger == null)
            trigger = control.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }

}
