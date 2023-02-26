using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallMonster : EnemyController
{
    public GameObject _fireBall;

    public Transform _fireballPos;

    private readonly float _fireSpeed = 100f;

    [SerializeField] private Material _ground;

    private void Start()
    {
        _ground.SetFloat("_Blend", 0);
    }

    public void OnParticleSystem()
    {
        GameObject fireBall = Instantiate(_fireBall, _fireballPos.position, Quaternion.identity);

        Vector3 forceVec = Instance.gameObject.transform.position - _fireballPos.transform.position;

        fireBall.GetComponent<Rigidbody>().AddForce(forceVec * _fireSpeed);

        RaycastHit hit;
        if (Physics.Raycast(_fireballPos.position, forceVec, out hit, Mathf.Infinity))
        {
            _ground.SetFloat("_Blend", 0.4f);
            _ground.SetFloat("_PointX", hit.collider.gameObject.transform.position.x);
            _ground.SetFloat("_PointZ", hit.collider.gameObject.transform.position.z);
            Invoke(nameof(DestroyCirle), 1);
        }

    }

    //ï\é¶ÇµÇΩçUåÇó\ë™â~Çè¡ÇµÇΩÇ¢
    void DestroyCirle()
    {
        _ground.SetFloat("_Blend", 0);
    }
}