using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallMonster : EnemyController
{
    public GameObject _fireBall;

    public Transform _fireballPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnParticleSystem()
    {
        GameObject fireBall = Instantiate(_fireBall, _fireballPos.position, Quaternion.identity);

        fireBall.GetComponent<Rigidbody>().AddForce(new Vector3(transform.forward.x, -1, transform.forward.z) * 500);
    }
}
