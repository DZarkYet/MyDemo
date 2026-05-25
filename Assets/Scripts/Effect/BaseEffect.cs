using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    public float duration;
    private float timer = 0f;

    private void OnEnable()
    {
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Recycle();
    }

    public virtual void Recycle()
    {
        this.gameObject.SetActive(false);
        PoolManager.Instance.PushObj(this.gameObject);
    }
}
