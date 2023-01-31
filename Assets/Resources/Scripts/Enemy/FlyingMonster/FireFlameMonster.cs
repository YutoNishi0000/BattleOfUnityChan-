using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlameMonster : EnemyController
{
    public ParticleSystem _fireflame;

    public Transform _fireflamePos;

    public void OnParticleSystem()
    {
        CreateParticleSystem(_fireflame, _fireflamePos, transform.rotation, 3.0f);
    }
}
