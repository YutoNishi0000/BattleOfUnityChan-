using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManagerScript : MonoBehaviour
{
    [SerializeField]
    public int _damage;     //�_���[�W��
    [SerializeField] private ParticleSystem _swordEffect;

    // Start is called before the first frame update
    void Start()
    {
        _damage = 50;
    }

    private void OnTriggerEnter(Collider other)
    {
        IMonsterDamageable monsterDamageable = other.gameObject.GetComponentInParent<IMonsterDamageable>();

        if(monsterDamageable == null)
        {
            Debug.Log("�C���^�[�t�F�C�X���擾�ł��܂���ł���");
            return;
        }

        //�G�t�F�N�g����
        Instantiate(_swordEffect, other.gameObject.transform.position, Quaternion.identity);

        bool counterAttack = GetComponentInParent<CharacterControlScript>()._counterCollider;

        int damage = counterAttack ? _damage * 3 : _damage;

        monsterDamageable.Damage(damage);
        Debug.Log("�U����^���܂���");
    }
}