using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccountData
{
    public AccountData() { }

    public List<UserInfo> userInfos = new List<UserInfo>();
}
