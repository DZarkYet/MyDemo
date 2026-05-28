using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBootStrap : MonoBehaviour
{
    public GameObject freeLookCamera;

    private void Awake()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Game_Start, () =>
        {
            CreatePlayer();
            LockMouse();
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Times_Up, () =>
        {
            UnLockMouse();
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Pause, () =>
        {
            UIManager.Instance.ShowPanel<QuitPanel>(E_UILayer.Top);
            UnLockMouse();
        });
        EventCenter.Instance.AddEventListener(E_EventType.E_Resume, () =>
        {
            UIManager.Instance.HidePanel<QuitPanel>();
            LockMouse();
        });
    }

    private Transform FindInAllChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindInAllChildren(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private void CreatePlayer()
    {
        ABResManager.Instance.LoadResAsync<GameObject>("player", "Player", (obj) =>
        {
            GameObject player = Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity);
            freeLookCamera.GetComponent<CinemachineFreeLook>().Follow = player.transform;
            Transform target = FindInAllChildren(player.transform, "Upper Chest");
            freeLookCamera.GetComponent<CinemachineFreeLook>().LookAt = target;
        });
    }


    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnLockMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

}
