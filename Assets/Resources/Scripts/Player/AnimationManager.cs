using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    //�U���̓����蔻��t���O
    private bool _attack;
    public GameObject SwordCollision;

    private void Start()
    {
        _attack = false;
        
    }

    public void AttackOnCollision()
    {
        _attack = true;
    }

    public void AttackOffCollision()
    {
        _attack = false;
    }
}
