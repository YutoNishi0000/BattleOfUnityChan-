using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastAnimationEvent : MonoBehaviour
{
    public GameObject[] _cols;

    private LastMonsterController _last;

    public int _damage;

    // Start is called before the first frame update
    void Start()
    {
        _damage = 0;

        _last = GetComponent<LastMonsterController>();

        for (int i = 0; i < _cols.Length; i++)
        {
            //_cols[i].SetActive(false);
            _cols[i].GetComponent<CapsuleCollider>().isTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�U�����̓����蔻����I����
    public void OnAttackCollider()
    {
        if (_last._attackInfo._attackState == LastMonsterController.AttackState.Attack1)
        {
            //�_���[�W�ʂ��Z�b�g
            _damage = _last._attackInfo._damage;
            //_cols[0].SetActive(true);
            _cols[0].GetComponent<CapsuleCollider>().isTrigger = true;
        }
        else if (_last._attackInfo._attackState == LastMonsterController.AttackState.Attack2)
        {
            //�_���[�W�ʂ��Z�b�g
            _damage = _last._attackInfo._damage;
            //_cols[1].SetActive(true);
            _cols[1].GetComponent<CapsuleCollider>().isTrigger = true;

            //�������U�����̓����蔻�肪�R��������
            if (_cols.Length == 3)
            {
                //_cols[2].SetActive(true);
                _cols[2].GetComponent<CapsuleCollider>().isTrigger = true;
            }
        }
    }

    //�U�����̓����蔻����I�t��
    public void OffAttackCollider()
    {
        if (_last._attackInfo._attackState == LastMonsterController.AttackState.Attack1)
        {
            //�_���[�W�ʂ��Z�b�g
            _damage = _last._attackInfo._damage;
            //_cols[0].SetActive(false);
            _cols[0].GetComponent<CapsuleCollider>().isTrigger = false;
        }
        else if (_last._attackInfo._attackState == LastMonsterController.AttackState.Attack2)
        {
            //�_���[�W�ʂ��Z�b�g
            _damage = _last._attackInfo._damage;
            _cols[1].GetComponent<CapsuleCollider>().isTrigger = false;

            //�������U�����̓����蔻�肪�R��������
            if (_cols.Length == 3)
            {
                _cols[2].GetComponent<CapsuleCollider>().isTrigger = false;
            }
        }
    }
}
