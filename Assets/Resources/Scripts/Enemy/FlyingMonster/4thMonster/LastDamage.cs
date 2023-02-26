using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastDamage : MonoBehaviour
{
    private LastAttackCollider _last;

    // Start is called before the first frame update
    void Start()
    {
        _last = FindObjectOfType<LastAttackCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _last = FindObjectOfType<LastAttackCollider>();

        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_last._damage);
            damageable.ShakeUI();
            Debug.Log("プレイヤーに攻撃");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        _last = FindObjectOfType<LastAttackCollider>();

        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            damageable.Damage(_last._damage);
            damageable.ShakeUI();
            Debug.Log("プレイヤーに攻撃");
        }
    }
}
