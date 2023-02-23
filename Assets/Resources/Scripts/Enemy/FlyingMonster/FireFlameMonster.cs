using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlameMonster : EnemyController
{
    public ParticleSystem _fireflame;

    public Transform _fireflamePos;

    public void OnParticleSystem()
    {
        float rad = 60 * Mathf.Deg2Rad;
        GameSystem.Instance._shake.Shake(3f, 0.1f, 1);
        Quaternion rotation = Quaternion.Euler(new Vector3(_fireflamePos.transform.forward.x * Mathf.Cos(rad), _fireflamePos.transform.forward.y, Mathf.Abs(_fireflamePos.transform.forward.z)));
        CreateParticleSystem(_fireflame, _fireflamePos, /*rotation*//*Quaternion.identity*/_fireflamePos.transform.rotation, 3.0f);
        Debug.Log(_fireflamePos.transform.rotation);
    }
}
