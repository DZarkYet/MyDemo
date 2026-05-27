using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : SingletonAutoMono<PauseManager>
{
    public bool isPaused { get; private set; }
    private int currentTimerID = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (currentTimerID != -1)
            TimerManager.Instance.StopTimer(currentTimerID);

        // 关闭玩家输入
        InputManager.Instance.StartOrCloseInputMgr(false);

        // 通知所有模块暂停
        EventCenter.Instance.EventTrigger(E_EventType.E_Pause);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // 恢复计时器
        if (currentTimerID != -1)
            TimerManager.Instance.StartTimer(currentTimerID);

        // 恢复玩家输入
        InputManager.Instance.StartOrCloseInputMgr(true);

        // 通知所有模块恢复
        EventCenter.Instance.EventTrigger(E_EventType.E_Resume);

    }

    public void SetTimerID(int id) => currentTimerID = id;
}
