using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastEffects : EnemyController
{
    public ParticleSystem[] _fireflame;

    public Transform _fireflamePos;

    private int _fireFlameCount;          //何回フレーム攻撃を行ったか

    private void Start()
    {
        _fireFlameCount = 0;
    }

    public void OnParticleSystem()
    {
        _fireFlameCount++;

        float rad = 60 * Mathf.Deg2Rad;
        Quaternion rotation = Quaternion.Euler(new Vector3(_fireflamePos.transform.forward.x * Mathf.Cos(rad), _fireflamePos.transform.forward.y, Mathf.Abs(_fireflamePos.transform.forward.z)));
        CreateParticleSystem(_fireflame[_fireFlameCount % 2], _fireflamePos, _fireflame[_fireFlameCount % 2].transform.rotation, 3.0f);
        Debug.Log(_fireflamePos.transform.rotation);
        GameSystem.Instance._shake.Shake(3f, 0.2f, 1f);
        Debug.Log("今のカウントは" + _fireFlameCount % 2);
        Debug.Log(_fireflame[_fireFlameCount % 2].name);
    }

    public void OnNomalParticleSystem()
    {
        CreateParticleSystem(_fireflame[2], _fireflamePos, _fireflamePos.transform.rotation, 3.0f);
        GameSystem.Instance._shake.Shake(3f, 0.2f, 1f);
    }

    public int GetSide()
    {
        return _fireFlameCount % 2;
    }

    public Quaternion GetFireFlameRotation()
    {
        return _fireflame[_fireFlameCount % 2].transform.rotation;
    }
}
