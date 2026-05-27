using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : SingletonAutoMono<LuaManager>
{
    private LuaEnv luaEnv;
    private bool isOpenABLoader = false;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (luaEnv != null)
            return;
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(MyCustomLoader);
        if(isOpenABLoader)
            luaEnv.AddLoader(MyCustomABLoader);
    }

    #region 加载lua脚本 重定向
    private byte[] MyCustomLoader(ref string fileName)
    {
        string path = Application.streamingAssetsPath + "/Lua/" + fileName + ".lua";

        if (File.Exists(path))
            return File.ReadAllBytes(path);
        else
            Debug.Log("MyCustomLoader重定向失败，文件名为" + path);
        return null;
    }

    private byte[] MyCustomABLoader(ref string fileName)
    {
        TextAsset lua = null;
        ABResManager.Instance.LoadResAsync<TextAsset>("lua", fileName + ".lua", (obj) =>
        {
            lua = obj;
        });
        if (lua != null)
            return lua.bytes;
        else
            Debug.Log("MyCustomABLoader重定向失败，文件名为：" + fileName);

        return null;
    }
    #endregion


    public LuaTable DoFile(string fileName)
    {
        string path = Application.streamingAssetsPath + "/Lua/" + fileName + ".lua";

        if (File.Exists(path))
        {
            var result = luaEnv.DoString(File.ReadAllText(path));
            return result[0] as LuaTable;
        }
        else
        {
            Debug.LogWarning($"[LuaManager] 找不到: {path}");
            return null;
        }
        
    }

    public void DoString(string str)
    {
        if (luaEnv == null)
            return;
        luaEnv.DoString(str);
    }

    /// <summary>
    /// 释放lua垃圾
    /// </summary>
    public void Tick()
    {
        if (luaEnv == null)
            return;
        luaEnv.Tick();
    }

    /// <summary>
    /// 销毁lua环境
    /// </summary>
    public void Dipose()
    {
        if (luaEnv == null)
            return;
        luaEnv.Dispose();
        luaEnv = null;
    }
}
