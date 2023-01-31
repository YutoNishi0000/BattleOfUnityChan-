using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManagerScript : MonoBehaviour
{
    [SerializeField]
    public int _damage;     //�_���[�W��
    bool _attackLock;

    // Start is called before the first frame update
    void Start()
    {
        _damage = 50;
        _attackLock = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_attackLock)
        {
            return;
        }

        IMonsterDamageable monsterDamageable = other.gameObject.GetComponentInParent<IMonsterDamageable>();

        if(monsterDamageable == null)
        {
            Debug.Log("�C���^�[�t�F�C�X���擾�ł��܂���ł���");
            return;
        }

        Debug.Log("�U����������܂���");

        bool counterAttack = GetComponentInParent<CharacterControlScript>()._counterCollider;

        Debug.Log("�J�E���^�[�t���O�F" + counterAttack); 

        int damage = counterAttack ? _damage * 3 : _damage;

        if(counterAttack)
        {
            Debug.Log("�J�E���^�[�A�^�b�N���y�􂾂�������l");
        }

        monsterDamageable.Damage(damage);
        Debug.Log("�U����^���܂���");

        StartCoroutine("CUnbeatableTime");
    }

    IEnumerator CUnbeatableTime()
    {
        _attackLock = true;
        yield return new WaitForSeconds(3);
        _attackLock = false;
    }
}