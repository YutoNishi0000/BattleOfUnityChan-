using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FireBallパーティクルシステムの衝突時の処理を記述する
public class FireBallController : EnemyController
{
    public ParticleSystem _explosion;
    public ParticleSystem _fire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground")/*other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("AttackCollider")*/)
        {
            Debug.Log("消す処理を行いません");
            GameSystem.Instance._shake.Shake(0.8f, 0.1f, 1);
            CreateParticleSystem2(_explosion, transform.position, Quaternion.identity, 3.0f);
            CreateParticleSystem2(_fire, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z), Quaternion.identity, 7.0f);
            Destroy(gameObject);
        }
    }
}
