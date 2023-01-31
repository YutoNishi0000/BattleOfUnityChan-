using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDetection : MonoBehaviour
{
    private FlyingMonster _flying;
    public bool _moveLock;
    Monster _monster;

    // Start is called before the first frame update
    void Start()
    {
        _flying = GetComponentInParent<FlyingMonster>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _flying._endScream && _flying._navMeshAgent.enabled)
        {
            Debug.Log("Player���m");
            //��ԋ߂��v���C���[��ړI�n��
            _flying._navMeshAgent.destination = FetchNearObjectWithTag("Player").transform.position;
        }
    }

    GameObject FetchNearObjectWithTag(string tagName)
    {
        // �Y���^�O��1���������ꍇ�͂����Ԃ�
        var targets = GameObject.FindGameObjectsWithTag(tagName);
        if (targets.Length == 1) return targets[0];

        GameObject result = null;
        var minTargetDistance = float.MaxValue;
        foreach (var target in targets)
        {
            // �O��v�������I�u�W�F�N�g�����߂��ɂ���΋L�^
            var targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (!(targetDistance < minTargetDistance)) continue;
            minTargetDistance = targetDistance;
            result = target.transform.gameObject;
        }

        // �Ō�ɋL�^���ꂽ�I�u�W�F�N�g��Ԃ�
        return result;
    }
}
