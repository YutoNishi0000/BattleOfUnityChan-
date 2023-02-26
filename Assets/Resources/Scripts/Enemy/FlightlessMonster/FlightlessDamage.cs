using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightlessDamage : MonoBehaviour
{
    private FirstAttackCollider _first;

    // Start is called before the first frame update
    void Start()
    {
        _first = FindObjectOfType<FirstAttackCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _first = FindObjectOfType<FirstAttackCollider>();

        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_first._damage);
            damageable.ShakeUI();
            Debug.Log("�v���C���[�ɍU��");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        _first = FindObjectOfType<FirstAttackCollider>();

        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            damageable.Damage(_first._damage);
            damageable.ShakeUI();
            Debug.Log("�G�t�F�N�g�ōU��");
        }
    }
}
