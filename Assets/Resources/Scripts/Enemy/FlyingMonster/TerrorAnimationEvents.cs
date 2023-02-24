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

    //攻撃時の当たり判定をオンに
    public void OnAttackCollider()
    {
        if (!_isFlightless)
        {
            if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack1)
            {
                //ダメージ量をセット
                _damage = _flying._attackInfo._damage;
                //_cols[0].SetActive(true);
                _cols[0].GetComponent<Collider>().enabled = true;
            }
            else if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack2)
            {
                //ダメージ量をセット
                _damage = _flying._attackInfo._damage;
                //_cols[1].SetActive(true);
                _cols[1].GetComponent<Collider>().enabled = true;

                //もしも攻撃時の当たり判定が３つあったら
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
                //ダメージ量をセット
                Debug.Log("攻撃の当たり判定オン");
                _damage = _flightless._attackInfo._damage;
                //_cols[0].SetActive(true);
                _cols[0].GetComponent<Collider>().enabled = true;
            }
            else if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack2)
            {
                //ダメージ量をセット
                _damage = _flightless._attackInfo._damage;
                _cols[1].GetComponent<Collider>().enabled = true;

                //もしも攻撃時の当たり判定が３つあったら
                if (_cols.Length == 3)
                {
                    _cols[2].GetComponent<Collider>().enabled = true;
                }
            }
            else if(_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack3)
            {
                //ダメージ量をセット
                Debug.Log("攻撃の当たり判定オン");
                _damage = _flightless._attackInfo._damage;
                _cols[0].GetComponent<Collider>().enabled = true;
            }
        }
    }

    //攻撃時の当たり判定をオフに
    public void OffAttackCollider()
    {
        if (!_isFlightless)
        {
            if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack1)
            {
                //ダメージ量をセット
                _damage = _flying._attackInfo._damage;
                //_cols[0].SetActive(false);
                _cols[0].GetComponent<Collider>().enabled = false;
            }
            else if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack2)
            {
                //ダメージ量をセット
                _damage = _flying._attackInfo._damage;
                _cols[1].GetComponent<Collider>().enabled = false;

                //もしも攻撃時の当たり判定が３つあったら
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
                //ダメージ量をセット
                _damage = _flightless._attackInfo._damage;
                _cols[0].GetComponent<Collider>().enabled = false;
            }
            else if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack2)
            {
                //ダメージ量をセット
                _damage = _flightless._attackInfo._damage;
                _cols[1].GetComponent<Collider>().enabled = false;

                //もしも攻撃時の当たり判定が３つあったら
                if (_cols.Length == 3)
                {
                    _cols[2].GetComponent<Collider>().enabled = false;
                }
            }
            else if (_flightless._attackInfo._attackState == FlightlessMonster.AttackState.Attack3)
            {
                //ダメージ量をセット
                Debug.Log("攻撃の当たり判定オフ");
                _cols[0].GetComponent<Collider>().enabled = false;
            }
        }
    }
}