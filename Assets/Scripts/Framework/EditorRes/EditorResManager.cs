using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/// <summary>
/// 编辑器资源管理器
/// 注意 ： 只有开发时能用该管理器加载资源
/// 发布后无法使用 因为它使用的是编辑器相关知识
/// </summary>
public class EditorResManager : BaseManager<EditorResManager>
{
    //用于放置需要打包进AB包的资源路径
    private string rootPath = "Assets/Editor/ArtRes/";

    private EditorResManager() { }

    //1.加载单个资源
    public T LoadEditorRes<T>(string path, bool isMp3 = true) where T : Object
    {
#if UNITY_EDITOR
        //预制体、纹理、图片
        string suffixName = "";
        if(typeof(T) == typeof(GameObject))
            suffixName = ".prefab";
        else if(typeof(T) == typeof(Sprite))
            suffixName = ".png";
        else if(typeof(T) == typeof(Material))
            suffixName = ".mat";
        else if(typeof(T) == typeof(AudioClip))
        {
            if (isMp3)
                suffixName = ".mp3";
            else
                suffixName = ".wav";
        }
        else if(typeof(T) == typeof(Texture2D))
            suffixName = ".texture";

            T res = AssetDatabase.LoadAssetAtPath<T>(rootPath + path + suffixName);
        return res;
#else
        return null;
#endif
    }

    //2.加载图集相关
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        //加载图集中的所有子资源
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //遍历所有子资源 得到同名资源返回
        foreach(var item in sprites)
        {
            if(item.name == spriteName)
            {
                return item as Sprite;
            }
        }
        return null;
#else
        return null;
#endif
    }

    //加载图集文件中的所有子图片 并返回字典
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string,Sprite> spriteDic = new Dictionary<string,Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (var item in sprites)
        {
            spriteDic.Add(item.name, item as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif
    }
}
