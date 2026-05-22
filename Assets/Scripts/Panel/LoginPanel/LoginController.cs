using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public LoginModel model;

    private void Awake()
    {
        model = gameObject.GetComponent<LoginModel>();
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
}
