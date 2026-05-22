using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 序列化和反序列化时使用哪种Json工具
/// </summary>
public enum JsonType
{
    JsonUtility,
    LitJson,
}

public class JsonManager : BaseManager<JsonManager>
{

    private JsonManager() { }

    /// <summary>
    /// 写入指定文件中的数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fileName"></param>
    /// <param name="type"></param>
    public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson)
    {
        //确定存储路径
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        //序列化得到json字符串
        string jsonStr = "";
        switch(type)
        {
            case JsonType.JsonUtility:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = LitJson.JsonMapper.ToJson(data);
                break;
        }
        //把序列化的json字符串 存储到指定文件当中
        File.WriteAllText(path, jsonStr);
    }

    /// <summary>
    /// 读取指定文件中的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : class, new()
    {
        //确定从哪个路径读取
        //首先先判断默认数据文件夹中是否有我们想要的数据 如果有 就从中获取
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";

        //先判断是否存在这个文件
        //如果不存在默认文件 就从读写文件夹中寻找
        if (!File.Exists(path))
            path = Application.persistentDataPath + "/" + fileName + ".json";
        //如果读写文件夹中还没有 那就返回一个默认对象
        if (!File.Exists(path))
            return null;

        string jsonStr = File.ReadAllText(path);
        //数据对象
        T data = null;
        //进行反序列化
        switch (type)
        {
            case JsonType.JsonUtility:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
        }

        //把对象返还出去
        return data;
    }
}
