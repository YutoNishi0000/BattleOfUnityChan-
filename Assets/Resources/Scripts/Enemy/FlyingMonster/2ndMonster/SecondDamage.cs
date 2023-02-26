using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondDamage : MonoBehaviour
{
    private SecondAttackCollider _second;

    // Start is called before the first frame update
    void Start()
    {
        _second = FindObjectOfType<SecondAttackCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _second = FindObjectOfType<SecondAttackCollider>();

        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_second._damage);
            damageable.ShakeUI();
            Debug.Log("�v���C���[�ɍU��");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        _second = FindObjectOfType<SecondAttackCollider>();

        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            damageable.Damage(_second._damage);
            damageable.ShakeUI();
            Debug.Log("�G�t�F�N�g�ōU��");
        }
    }
}
