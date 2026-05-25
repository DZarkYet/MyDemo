using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainModel : MonoBehaviour
{
    public RecordData GetData()
    {
        RecordData data = JsonManager.Instance.LoadData<RecordData>("RecordData", JsonType.JsonUtility);
        return data;
    }
}
