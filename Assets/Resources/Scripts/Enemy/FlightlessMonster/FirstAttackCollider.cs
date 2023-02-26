using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAttackCollider : MonoBehaviour
{
    public GameObject _col;

    private FlightlessMonster _first;
    public bool _isFlightless;

    public float _damage;

    private void Start()
    {
        _damage = 0;

        _first = GetComponent<FlightlessMonster>();
        _col.GetComponent<Collider>().enabled = false;
    }

    //攻撃時の当たり判定をオンに
    public void OnAttackCollider()
    {
        _col.GetComponent<Collider>().enabled = true;
        _damage = _first._attackInfo._damage;
    }

    //攻撃時の当たり判定をオフに
    public void OffAttackCollider()
    {
        _col.GetComponent<Collider>().enabled = false;
    }
}
