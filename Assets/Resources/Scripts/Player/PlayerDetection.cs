using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private CharacterControlScript _player;

    private void Start()
    {
        _player = GetComponentInParent<CharacterControlScript>();
        if(_player == null)
        {
            Debug.Log("Player�̎擾�Ɏ��s");
        }
        else
        {
            Debug.Log("Player�擾�I�I");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(_player == null)
            {
                Debug.Log("Player�̎擾�Ɏ��s");
                return;
            }
            _player._avoidance = false;
            Debug.Log("�ǂɂԂ���܂���");
        }
        else
        {
            return;
        }
    }
}
