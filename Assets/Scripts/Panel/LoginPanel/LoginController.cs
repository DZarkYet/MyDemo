using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public LoginModel model;
    private float progress;

    private void Awake()
    {
        model = gameObject.GetComponent<LoginModel>();
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Scene_Load, GetProgress);
    }

    public bool CheckAcount(string username, string password)
    {

        AccountData data = model.CheckLoginData(username, password);
        if(data != null)
        {
            foreach (var item in data.userInfos)
            {
                if (item.username == username && item.password == password)
                    return true;
            }
            return false;
        }
        else
            return false;
    }

    public void SwitchScene(string sceneName)
    {
        ScenesManager.Instance.LoadSceneAsync(sceneName, () =>
        {
            UIManager.Instance.HidePanel<LoadingPanel>();
            UIManager.Instance.HidePanel<LoginPanel>();
            UIManager.Instance.ShowPanel<MainPanel>();
            UIManager.Instance.ShowPanel<WarningPanel>(E_UILayer.Top, (obj) =>
            {
                obj.contentText.text = "wasd¿ØÖÆ½ÇÉ«£¬×óshift³å´Ì£¬Êó±ê×ó¼ü¹¥»÷£¬ÓÒ¼üÉÁ±Ü£¬¿Õ¸ñ¼üÌøÔ¾";
            });
        });
    }

    private void GetProgress(float progress)
    {
        this.progress = progress;
        UIManager.Instance.GetPanel<LoadingPanel>((obj) =>
        {
            obj.progressSlider.value = progress;
        });
    }
}
