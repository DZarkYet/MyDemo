using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistryPanel : BasePanel
{
    public RegistryController controller;

    private InputField usernameField;
    private InputField passwordField;
    private InputField passwordField2;
    private Button confirmBtn;

    protected override void Awake()
    {
        base.Awake();

        controller = this.gameObject.GetComponent<RegistryController>();
        usernameField = GetControl<InputField>("UsernameField");
        passwordField = GetControl<InputField>("PasswordField");
        passwordField2 = GetControl<InputField>("PasswordField2");
        confirmBtn = GetControl<Button>("ConfirmBtn");
    }

    protected override void ClickBtn(string btnName)
    {
        if (btnName == "ConfirmBtn")
        {
            int checkResult = controller.CheckAcount(usernameField.text, passwordField.text, passwordField2.text);
            switch (checkResult)
            {
                case 0:
                    UIManager.Instance.ShowPanel<WarningPanel>(E_UILayer.Top, (obj) =>
                    {
                        obj.contentText.text = "两次输入的密码不相同，请重新输入";
                    });
                    break;
                case 1:
                    UIManager.Instance.ShowPanel<WarningPanel>(E_UILayer.Top, (obj) =>
                    {
                        obj.contentText.text = "该账号已创建";
                    });
                    break;
                case 2:
                    UIManager.Instance.ShowPanel<WarningPanel>(E_UILayer.Top, (obj) =>
                    {
                        EventCenter.Instance.AddEventListener(E_EventType.E_Close_Registry, HideMe);
                        obj.contentText.text = "新账号创建成功，可直接登录";
                    });
                    break;
            }
        }
        else if(btnName == "BackBtn")
            HideMe();
    }

    public override void HideMe()
    {
        this.gameObject.SetActive(false);
    }

    public override void ShowMe()
    {
        this.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Close_Registry, HideMe);
    }
}
