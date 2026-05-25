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

    protected override void Awake()
    {
        base.Awake();
        startBtn = GetControl<Button>("StartBtn");
        timeText = GetControl<Text>("TimeText");
        hpBar = GetControl<Slider>("HpBar");
        hpBar.value = 1;
        timeText.text = "30:00";
        controller.ShowRank();
    }

    protected override void ClickBtn(string btnName)
    {
        if(btnName == "StartBtn")
        {
            startBtn.gameObject.SetActive(false);
            controller.GenerateEnemy();
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

}
