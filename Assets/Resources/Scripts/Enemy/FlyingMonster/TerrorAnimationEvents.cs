using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrorAnimationEvents : MonoBehaviour
{
    public GameObject[] _cols;

    private FlyingMonster _flying;
    private FlightlessMonster _flightless;
    public bool _isFlightless;

    public float _damage;

    private void Start()
    {
        _damage = 0;
        if (!_isFlightless)
        {
            _flying = GetComponent<FlyingMonster>();
        }
        else
        {
            _flightless = GetComponent<FlightlessMonster>();
        }

        for(int i = 0; i < _cols.Length; i++)
        {
            //_cols[i].SetActive(false);
            _cols[i].GetComponent<Collider>().enabled = false;
        }
    }

    //�U�����̓����蔻����I����
    public void OnAttackCollider()
    {
        if (!_isFlightless)
        {
            if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack1)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flying._attackInfo._damage;
                //_cols[0].SetActive(true);
                _cols[0].GetComponent<Collider>().enabled = true;
            }
            else if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack2)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flying._attackInfo._damage;
                //_cols[1].SetActive(true);
                _cols[1].GetComponent<Collider>().enabled = true;

                //�������U�����̓����蔻�肪�R��������
                if (_cols.Length == 3)
                {
                    //_cols[2].SetActive(true);
                    _cols[2].GetComponent<Collider>().enabled = true;
                }
            }
        }
        else
        {
            if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack1)
            {
                //�_���[�W�ʂ��Z�b�g
                Debug.Log("�U���̓����蔻��I��");
                _damage = _flightless._attackInfo._damage;
                //_cols[0].SetActive(true);
                _cols[0].GetComponent<Collider>().enabled = true;
            }
            else if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack2)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flightless._attackInfo._damage;
                _cols[1].GetComponent<Collider>().enabled = true;

                //�������U�����̓����蔻�肪�R��������
                if (_cols.Length == 3)
                {
                    _cols[2].GetComponent<Collider>().enabled = true;
                }
            }
            else if(_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack3)
            {
                //�_���[�W�ʂ��Z�b�g
                Debug.Log("�U���̓����蔻��I��");
                _damage = _flightless._attackInfo._damage;
                _cols[0].GetComponent<Collider>().enabled = true;
            }
        }
    }

    //�U�����̓����蔻����I�t��
    public void OffAttackCollider()
    {
        if (!_isFlightless)
        {
            if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack1)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flying._attackInfo._damage;
                //_cols[0].SetActive(false);
                _cols[0].GetComponent<Collider>().enabled = false;
            }
            else if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack2)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flying._attackInfo._damage;
                _cols[1].GetComponent<Collider>().enabled = false;

                //�������U�����̓����蔻�肪�R��������
                if (_cols.Length == 3)
                {
                    _cols[2].GetComponent<Collider>().enabled = false;
                }
            }
        }
        else
        {
            if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack1)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flightless._attackInfo._damage;
                _cols[0].GetComponent<Collider>().enabled = false;
            }
            else if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack2)
            {
                //�_���[�W�ʂ��Z�b�g
                _damage = _flightless._attackInfo._damage;
                _cols[1].GetComponent<Collider>().enabled = false;

                //�������U�����̓����蔻�肪�R��������
                if (_cols.Length == 3)
                {
                    _cols[2].GetComponent<Collider>().enabled = false;
                }
            }
            else if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack3)
            {
                //�_���[�W�ʂ��Z�b�g
                Debug.Log("�U���̓����蔻��I�t");
                _cols[0].GetComponent<Collider>().enabled = false;
            }
        }
    }
}