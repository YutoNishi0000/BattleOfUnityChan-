using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdDamage : MonoBehaviour
{
    private ThirdAttackCollider _terEve;

    // Start is called before the first frame update
    void Start()
    {
        _terEve = FindObjectOfType<ThirdAttackCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _terEve = FindObjectOfType<ThirdAttackCollider>();

        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_terEve._damage);
            damageable.ShakeUI();
            Debug.Log("�v���C���[�ɍU��");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        _terEve = FindObjectOfType<ThirdAttackCollider>();

        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            damageable.Damage(_terEve._damage);
            damageable.ShakeUI();
            Debug.Log("�G�t�F�N�g�ōU��");
        }
    }
}