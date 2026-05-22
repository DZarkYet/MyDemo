using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootStrap : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.ShowPanel<LoginPanel>(E_UILayer.Middle);
    }

}
