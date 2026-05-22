using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistryController : MonoBehaviour
{
    public RegistryModel model;

    private void Awake()
    {
        model = this.gameObject.GetComponent<RegistryModel>();
    }

    public int CheckAcount(string username, string password, string password2)
    {
        if (password != password2)
            return 0;
        AccountData data = model.CheckLoginData(username, password);
        if (data != null)
        {
            foreach (var item in data.userInfos)
            {
                if (item.username == username)
                    return 1;
            }
            UserInfo info = new UserInfo(username, password);
            data.userInfos.Add(info);
            JsonManager.Instance.SaveData(data, "AccountData", JsonType.JsonUtility);
            return 2;
        }
        else
        {
            UserInfo info = new UserInfo(username, password);
            data = new AccountData();
            data.userInfos.Add(info);
            JsonManager.Instance.SaveData(data, "AccountData", JsonType.JsonUtility);
            return 2;
        }
            
    }

}
