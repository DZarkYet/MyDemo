using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : BasePanel
{
    public Text contentText;
    private Button confirmBtn;

    protected override void Awake()
    {
        base.Awake();
        confirmBtn = GetControl<Button>("ConfirmBtn");
        contentText = GetControl<Text>("ContentText");
    }

    protected override void ClickBtn(string btnName)
    {
        HideMe();
        EventCenter.Instance.EventTrigger(E_EventType.E_Close_Registry);
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
