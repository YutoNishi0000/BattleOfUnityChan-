using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FireBall�p�[�e�B�N���V�X�e���̏Փˎ��̏������L�q����
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
            Debug.Log("�����������s���܂���");
            CreateParticleSystem2(_explosion, transform.position, Quaternion.identity, 3.0f);
            CreateParticleSystem2(_fire, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z), Quaternion.identity, 7.0f);
            Destroy(gameObject);
            //return;
        }

        //��������ɓ��������玩�g�����ł������G�t�F�N�g�����΂炭�̊ԕ\������
        //Destroy(gameObject);
        //Instantiate(_explosion, other.gameObject.transform.position, Quaternion.identity);
        //Instantiate(_fire, other.gameObject.transform.position, Quaternion.identity);
        //Destroy(_explosion, 10f);
        //Destroy(_fire, 15f);
    }
}