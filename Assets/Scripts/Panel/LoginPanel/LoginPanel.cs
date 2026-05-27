using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public LoginController controller;

    private InputField usernameField;
    private InputField passwordField;
    private Button loginBtn;
    private Button registryBtn;


    protected override void Awake()
    {
        base.Awake();
        controller = gameObject.GetComponent<LoginController>();
        usernameField = GetControl<InputField>("UsernameField");
        passwordField = GetControl<InputField>("PasswordField");
        loginBtn = GetControl<Button>("Btn_Login");
        registryBtn = GetControl<Button>("Btn_Registry");

    }

    protected override void ClickBtn(string btnName)
    {
        if(btnName == "Btn_Login")
        {
            if(controller.CheckAcount(usernameField.text, passwordField.text))
            {
                Debug.Log("µÇÂ¼³É¹¦");
                MusicManager.Instance.StopBKMusic();
                UIManager.Instance.ShowPanel<LoadingPanel>(E_UILayer.Top);
                controller.SwitchScene("MainScene");
            }
            else
            {
                Debug.Log("µÇÂ¼Ê§°Ü£¬Î´×¢²áÕË»§");
                UIManager.Instance.ShowPanel<WarningPanel>(E_UILayer.Top, (obj) =>
                {
                    obj.contentText.text = "Î´×¢²áÕËºÅ£¬ÇëÏÈ×¢²áÕËºÅºóµÇÂ¼";
                });
            }
        }
        else
            UIManager.Instance.ShowPanel<RegistryPanel>(E_UILayer.Middle);
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
