using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondAttackCollider : MonoBehaviour
{
    public GameObject _col;

    private SecondMonster _second;
    public bool _isFlightless;

    public float _damage;

    private void Start()
    {
        _damage = 0;

        _second = GetComponent<SecondMonster>();
        _col.GetComponent<Collider>().enabled = false;
    }

    //攻撃時の当たり判定をオンに
    public void OnAttackCollider()
    {
        _col.GetComponent<Collider>().enabled = true;
        _damage = _second._attackInfo._damage;
    }

    //攻撃時の当たり判定をオフに
    public void OffAttackCollider()
    {
        _col.GetComponent<Collider>().enabled = false;
    }
}
