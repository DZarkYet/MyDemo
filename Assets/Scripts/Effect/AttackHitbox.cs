using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    private void OnEnable()
    {
        hitTargets.Clear();   // 每次从池里拿出来清空命中记录，避免一个框打两次
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !hitTargets.Contains(other.gameObject))
        {
            hitTargets.Add(other.gameObject);
            other.GetComponent<EnemyController>()?.TakeDamage(damage);
        }
    }
}
