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
            Debug.Log("Player‚Ìæ“¾‚É¸”s");
        }
        else
        {
            Debug.Log("Playeræ“¾II");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(_player == null)
            {
                Debug.Log("Player‚Ìæ“¾‚É¸”s");
                return;
            }
            _player._avoidance = false;
            Debug.Log("•Ç‚É‚Ô‚Â‚©‚è‚Ü‚µ‚½");
        }
        else
        {
            return;
        }
    }
}
