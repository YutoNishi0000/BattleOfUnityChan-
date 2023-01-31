using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationEvents : MonoBehaviour
{
    public SABoneCollider[] _cols;

    private FlyingMonster _flying;

    FlyingMonster.AttackState _attackState;

    public int _damage;

    private void Start()
    {
        _damage = 0;
        _flying = GetComponent<FlyingMonster>();

        _attackState = new FlyingMonster.AttackState();

        for (int i = 0; i < _cols.Length; i++)
        {
            _cols[i].enabled = false;
        }
    }

    //攻撃時の当たり判定をオンに
    public void OnAttackCollider()
    {
        if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack1)
        {
            //ダメージ量をセット
            _damage = _flying._attackInfo._damage;
            _cols[0].enabled = true;
        }
        else if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack2)
        {
            //ダメージ量をセット
            _damage = _flying._attackInfo._damage;
            _cols[1].enabled = true;
        }
    }

    //攻撃時の当たり判定をオフに
    public void OffAttackCollider()
    {
        if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack1)
        {   
            _cols[0].enabled = false;
        }
        else if (_flying._attackInfo._attackState == FlyingMonster.AttackState.Attack2)
        {
            _cols[1].enabled = false;
        }
    }
}