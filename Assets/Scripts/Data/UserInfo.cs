using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string username;
    public string password;

    public UserInfo(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
