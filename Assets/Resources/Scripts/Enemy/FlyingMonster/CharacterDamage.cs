using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamage : MonoBehaviour
{
    BaseAnimationEvents _animEve;

    TerrorAnimationEvents _terEve;

    // Start is called before the first frame update
    void Start()
    {
        _terEve = FindObjectOfType<TerrorAnimationEvents>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_terEve._damage);
            damageable.ShakeUI();
            Debug.Log("プレイヤーに攻撃");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            damageable.Damage(_terEve._damage);
            damageable.ShakeUI();
            Debug.Log("エフェクトで攻撃");
        }
    }
}