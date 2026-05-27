using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : BasePanel
{
    private Button cancelBtn;
    private Button confirmBtn;
    public QuitController controller;

    protected override void Awake()
    {
        base.Awake();
        cancelBtn = GetControl<Button>("CancelBtn");
        confirmBtn = GetControl<Button>("ConfirmBtn");
    }

    protected override void ClickBtn(string btnName)
    {
        if (btnName == "CancelBtn")
        {
            HideMe();
            PauseManager.Instance.Resume();
        }   
        else if (btnName == "ConfirmBtn")
            controller.QuitGame();
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
