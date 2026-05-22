using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ConfigPath
{
    [MenuItem("Config/PrintPersistentPath")]
    public static void PrintPath()
    {
        Debug.Log(Application.persistentDataPath);
    }
}
