using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public Slider progressSlider;

    protected override void Awake()
    {
        base.Awake();
        progressSlider = GetControl<Slider>("ProgressSlider");
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
