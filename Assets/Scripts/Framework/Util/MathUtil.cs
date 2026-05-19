using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MathUtil
{
    #region 角度和弧度
    /// <summary>
    /// 角度转弧度的方法
    /// </summary>
    /// <param name="deg">角度值</param>
    /// <returns>弧度值</returns>
    public static float Deg2Rad(float deg)
    {
        return deg * Mathf.Deg2Rad;
    }

    /// <summary>
    /// 弧度转角度的方法
    /// </summary>
    /// <param name="rad">弧度值</param>
    /// <returns>角度值</returns>
    public static float Rad2Deg(float rad)
    {
        return rad * Mathf.Rad2Deg;
    }
    #endregion

    #region 距离计算
    /// <summary>
    /// 获取XZ平面上两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXZ(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(srcPos, targetPos);
    }

    /// <summary>
    /// 判断两点之间距离是否小于目标距离 XZ平面
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXZ(Vector3 srcPos, Vector3 targetPos,float dis)
    {
        return GetObjDistanceXZ(srcPos, targetPos) <= dis;
    }

    /// <summary>
    /// 获取XY平面上两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXY(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.z = 0;
        targetPos.z = 0;
        return Vector3.Distance(srcPos, targetPos);
    }

    /// <summary>
    /// 判断两点之间距离是否小于目标距离 XY平面
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXY(Vector3 srcPos, Vector3 targetPos, float dis)
    {
        return GetObjDistanceXY(srcPos, targetPos) <= dis;
    }


    #endregion

    #region 位置判断
    /// <summary>
    /// 判断世界坐标系下的某一个点 是否在屏幕范围外
    /// </summary>
    /// <param name="pos">世界坐标系下的一个点的位置</param>
    /// <returns>如果在可见范围外返回true 否则返回false</returns>
    public static bool IsWorldPosOutScreen(Vector3 pos)
    {
        //将世界坐标转为屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        //判断是否在屏幕范围内
        if (screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
            return false;
        return true;
    }

    /// <summary>
    /// 判断某一个位置是否在指定扇形范围内（注意：传入的坐标向量必须都是基于通过一个坐标系下）
    /// </summary>
    /// <param name="pos">扇形中心点位置</param>
    /// <param name="forward">自己的面朝向</param>
    /// <param name="targetPos">目标对象</param>
    /// <param name="radius">半径</param>
    /// <param name="angle">扇形角度</param>
    /// <returns></returns>
    public static bool IsInSectorRangeXZ(Vector3 pos, Vector3 forward, Vector3 targetPos, float radius, float angle)
    {
        pos.y = 0;
        forward.y = 0;
        targetPos.y = 0;
        //距离 + 角度
        return Vector3.Distance(pos, targetPos) <= radius && Vector3.Angle(forward, targetPos - pos) <= angle / 2f;
    }
    #endregion

    #region 射线检测

    #region 返回一个对象内容
    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数 将碰到的RaycastHit传出去</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, LayerMask layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo);
    }

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数 将碰到的GameObject传出去</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, UnityAction<GameObject> callBack, float maxDistance, LayerMask layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo.collider.gameObject);
    }

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数 将碰到的对象信息上挂载的指定脚本传出去</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance, LayerMask layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
    }
    #endregion

    #region 返回多个对象内容
    /// <summary>
    /// 射线检测获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数 将碰到的RaycastHit信息传出去 每一个对象会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">指定层级</param>
    public static void RayCastAll(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, LayerMask layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            callBack.Invoke(hitInfos[i]);
        }
    }

    /// <summary>
    /// 射线检测获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数 将碰到的GameObject传出去 每一个对象会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">指定层级</param>
    public static void RayCastAll(Ray ray, UnityAction<GameObject> callBack, float maxDistance, LayerMask layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            callBack.Invoke(hitInfos[i].collider.gameObject);
        }
    }

    /// <summary>
    /// 射线检测获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数 将碰到的对象信息上挂载的指定脚本传出去 每一个对象会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">指定层级</param>
    public static void RayCastAll<T>(Ray ray, UnityAction<T> callBack, float maxDistance, LayerMask layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            callBack.Invoke(hitInfos[i].collider.gameObject.GetComponent<T>());
        }
    }
    #endregion

    #endregion

    #region 范围检测
    /// <summary>
    /// 进行盒状范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息类型 可以填写 GameObject Collider 以及对象上依附的组件类型</typeparam>
    /// <param name="center">盒状范围中心点</param>
    /// <param name="rotation">盒子的角度</param>
    /// <param name="halfExtents">长宽高的一半</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    public static void OverlapBox<T>(Vector3 center, Quaternion rotation, Vector3 halfExtents,
        int layerMask, UnityAction<T> callBack) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation,
            layerMask, QueryTriggerInteraction.Collide);
        for(int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
                callBack.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 进行球体范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息类型 可以填写 GameObject Collider 以及对象上依附的组件类型</typeparam>
    /// <param name="center">球体中心点</param>
    /// <param name="radius">球体半径</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    public static void OverlapSphere<T>(Vector3 center, float radius, int layerMask, UnityAction<T> callBack) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
                callBack.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
    #endregion
}
