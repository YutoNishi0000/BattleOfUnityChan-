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
            Debug.Log("Playerの取得に失敗");
        }
        else
        {
            Debug.Log("Player取得！！");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(_player == null)
            {
                Debug.Log("Playerの取得に失敗");
                return;
            }
            _player._avoidance = false;
            Debug.Log("壁にぶつかりました");
        }
        else
        {
            return;
        }
    }
}
