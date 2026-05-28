using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public MainModel model;
    private RecordData data;
    public Transform rankPanel;
    public MainPanel mainPanel;

    public void StartTimer()
    {
        int timerID = TimerManager.Instance.CreateTimer(true, 30000, () =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_Times_Up);
            WriteData();
            ShowRank();
            Debug.Log("计时结束, Instance 对象ID = " + GameManager.Instance.GetInstanceID() + ", killCount = " + GameManager.Instance.killCount);
            UIManager.Instance.ShowPanel<WarningPanel>(E_UILayer.Top, (obj) =>
            {
                obj.contentText.text = "计时结束，成功击杀" + GameManager.Instance.killCount.ToString() + "个敌人";
            });
        }, 1000, () =>
        {
            mainPanel.nowTime -= 1000;
        });
        PauseManager.Instance.SetTimerID(timerID);
    }

    public void ShowRank()
    {
        if(rankPanel.childCount > 0)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in rankPanel)
                children.Add(child);
            foreach (Transform child in children)
                Destroy(child.gameObject);
            children = null;
        }
        data = model.GetData();
        if(data != null)
        {
            data.recordData.Sort((a, b) => b.count.CompareTo(a.count));
            foreach(var item in data.recordData)
            {
                GameObject cell = PoolManager.Instance.GetObj("Cell");
                cell.transform.Find("DateText").GetComponent<Text>().text = "日期：" + item.date;
                cell.transform.Find("CountText").GetComponent<Text>().text = "击杀：" + item.count;
                cell.transform.SetParent(rankPanel, false);
            }
        }
            
    }

    public void WriteData()
    {
        RecordInfo info = new RecordInfo();
        DateTime now = DateTime.Now;
        info.date = now.Year.ToString() + ":" + now.Month.ToString() + ":" + now.Day.ToString();
        info.count = GameManager.Instance.killCount;
        data = model.GetData();
        if(data == null)
            data = new RecordData();
        data.recordData.Add(info);
        JsonManager.Instance.SaveData(data, "RecordData", JsonType.JsonUtility);
    }

    public void StartGame()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_Game_Start);
    }
}
