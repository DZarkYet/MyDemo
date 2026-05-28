using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    public MainController controller;
    private Button startBtn;
    private Text timeText;
    private Slider hpBar;
    private float maxHp = 300f;
    public float nowTime = 30000;

    protected override void Awake()
    {
        base.Awake();
        startBtn = GetControl<Button>("StartBtn");
        timeText = GetControl<Text>("TimeText");
        hpBar = GetControl<Slider>("HpBar");
        hpBar.value = 1;
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Player_Hit, ChangeHpBar);
        EventCenter.Instance.AddEventListener(E_EventType.E_Times_Up, () =>
        {
            startBtn.gameObject.SetActive(true);
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Player_Dead, () =>
        {
            startBtn.gameObject.SetActive(true);
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Game_Start, () =>
        {
            hpBar.value = 1;
        });
        timeText.text = (nowTime / 1000).ToString();
        controller.ShowRank();
    }

    private void Update()
    {
        timeText.text = (nowTime / 1000).ToString();
    }

    protected override void ClickBtn(string btnName)
    {
        if(btnName == "StartBtn")
        {
            startBtn.gameObject.SetActive(false);
            nowTime = 30000;
            controller.StartGame();
            controller.StartTimer();
        }
    }

    public override void HideMe()
    {
        this.gameObject.SetActive(false);
    }

    public override void ShowMe()
    {
        this.gameObject.SetActive(true);
    }

    private void ChangeHpBar(float nowHp)
    {
        hpBar.value = nowHp / maxHp;
    }
}
