using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public MainModel model;
    private RecordData data;
    private Transform rankPanel;

    public void StartTimer()
    {
        TimerManager.Instance.CreateTimer(true, 30000, () =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Times_Up);
            TimerManager.Instance.RemoveTimer(1);
        });
    }

    public void ShowRank()
    {
        data = model.GetData();
        if(data != null)
        {
            data.recordData.Sort((a, b) => a.count.CompareTo(b.count));
            foreach(var item in data.recordData)
            {
                GameObject cell = PoolManager.Instance.GetObj("Cell");
                cell.transform.Find("DateText").GetComponent<Text>().text = "ÈÕÆÚ£º" + item.date;
                cell.transform.Find("CountText").GetComponent<Text>().text = "»÷É±£º" + item.count;
                cell.transform.SetParent(rankPanel);
            }
        }
            
    }

    public void WriteData()
    {

    }

    public void GenerateEnemy()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_Game_Start);
    }
}
