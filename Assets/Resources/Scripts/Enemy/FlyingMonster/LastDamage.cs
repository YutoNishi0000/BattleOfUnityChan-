using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastDamage : MonoBehaviour
{
    private LastAnimationEvent _last;

    // Start is called before the first frame update
    void Start()
    {
        _last = GetComponentInParent<LastAnimationEvent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_last._damage);
            Debug.Log("ÉvÉåÉCÉÑÅ[Ç…çUåÇ");
        }
    }
}
