using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBootStrap : MonoBehaviour
{
    public GameObject freeLookCamera;

    private void Awake()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_Game_Start, LockMouse);
        EventCenter.Instance.AddEventListener(E_EventType.E_Times_Up, RemoveListenr);
        ABResManager.Instance.LoadResAsync<GameObject>("player", "Player", (obj) =>
        {
            GameObject player = Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity);
            freeLookCamera.GetComponent<CinemachineFreeLook>().Follow = player.transform;
            Transform target = FindInAllChildren(player.transform, "Upper Chest");
            freeLookCamera.GetComponent<CinemachineFreeLook>().LookAt = target;
        });
    }

    private void Start()
    {
        
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

    void Update()
    {
        
    }

    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InputManager.Instance.ChangeKeyInfo(E_EventType.E_Mouse_UnLock, KeyCode.LeftAlt, InputInfo.E_InputType.Down);
        InputManager.Instance.ChangeKeyInfo(E_EventType.E_Mouse_Lock, KeyCode.LeftAlt, InputInfo.E_InputType.Up);
        EventCenter.Instance.AddEventListener(E_EventType.E_Mouse_UnLock, UnLockMouse);
        EventCenter.Instance.AddEventListener(E_EventType.E_Mouse_Lock, LockMouse);
    }

    private void UnLockMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void RemoveListenr()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Mouse_UnLock, UnLockMouse);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Mouse_Lock, LockMouse);
    }
}
