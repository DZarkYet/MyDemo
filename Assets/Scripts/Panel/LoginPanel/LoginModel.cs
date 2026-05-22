using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginModel : MonoBehaviour
{
    public AccountData CheckLoginData(string username, string password)
    {
        AccountData data = JsonManager.Instance.LoadData<AccountData>("AccountData", JsonType.JsonUtility);
        return data;
    }
}
