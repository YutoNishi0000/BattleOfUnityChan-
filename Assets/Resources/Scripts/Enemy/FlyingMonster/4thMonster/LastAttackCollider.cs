using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastAttackCollider : MonoBehaviour
{
    public GameObject[] _col;

    private LastMonster _last;

    public float _damage;

    public GameObject[] _fireCols;

    private void Start()
    {
        _damage = 0;

        _last = GetComponent<LastMonster>();
        for(int i = 0; i < _col.Length; i++)
        {
            _col[i].GetComponent<Collider>().enabled = false;
        }

        for (int i = 0; i < _fireCols.Length; i++)
        {
            _fireCols[i].GetComponent<Collider>().enabled = false;
        }
    }

    //�U�����̓����蔻����I����
    public void OnAttackCollider(int i)
    {
        _col[i].GetComponent<Collider>().enabled = true;
        _damage = _last._attackInfo._damage;
    }

    //�U�����̓����蔻����I�t��
    public void OffAttackCollider(int i)
    {
        _col[i].GetComponent<Collider>().enabled = false;
    }

    public void StartFlyingAttack()
    {
        Debug.Log("yobaretatatatatatataatatatatatatatatata");
        switch (GetComponent<LastEffects>().GetSide())
        {
            case 0:
                _fireCols[0].GetComponent<Collider>().enabled = true;
                Debug.Log("�t�@�C���[�R���W����1�I��");
                break;
            case 1:
                _fireCols[1].GetComponent<Collider>().enabled = true;
                Debug.Log("�t�@�C���[�R���W����2�I��");
                break;
        }
    }

    public void EndFlyingAttack()
    {
        for (int i = 0; i < _fireCols.Length; i++)
        {
            _fireCols[i].GetComponent<Collider>().enabled = false;
            Debug.Log("�t�@�C���[�R���W�����I�t");
        }
    }

}
