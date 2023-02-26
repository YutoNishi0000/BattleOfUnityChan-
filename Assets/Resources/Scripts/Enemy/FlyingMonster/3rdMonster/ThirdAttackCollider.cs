using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdAttackCollider : MonoBehaviour
{
    public GameObject _col;

    private ThirdMonster _second;
    public bool _isFlightless;

    public float _damage;

    private void Start()
    {
        _damage = 0;

        _second = GetComponent<ThirdMonster>();
        _col.GetComponent<Collider>().enabled = false;
    }

    //�U�����̓����蔻����I����
    public void OnAttackCollider()
    {
        _col.GetComponent<Collider>().enabled = true;
        _damage = _second._attackInfo._damage;
    }

    //�U�����̓����蔻����I�t��
    public void OffAttackCollider()
    {
        _col.GetComponent<Collider>().enabled = false;
    }
}