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
        _terEve = GetComponentInParent<TerrorAnimationEvents>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable.Damage(_terEve._damage);
            Debug.Log("ÉvÉåÉCÉÑÅ[Ç…çUåÇ");
        }
    }
}