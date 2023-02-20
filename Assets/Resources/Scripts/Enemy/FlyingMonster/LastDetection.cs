using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LastDetection : EnemyController
{
    private LastMonsterController _lastMonster;
    public bool _moveLock;
    Monster _monster;
    private GameObject flyingPos;
    //private GameObject landPos;

    // Start is called before the first frame update
    void Start()
    {
        _lastMonster = GetComponentInParent<LastMonsterController>();
        flyingPos = GameObject.Find("flyingPosition");
        //landPos = GameObject.Find("landPosition");
    }

    private void Update()
    {
        if (_lastMonster._endScream && !_lastMonster._landing && _lastMonster._navMeshAgent.velocity.magnitude < 0.5f && !_lastMonster.a && _lastMonster._flyingMove)
        {
            _lastMonster._navMeshAgent.enabled = false;
            _lastMonster.transform.DOMove(flyingPos.transform.position, 3);
        }
        else if(_lastMonster._endScream && _lastMonster._navMeshAgent.enabled && _lastMonster._landing)
        {
            _lastMonster._navMeshAgent.destination = Instance.gameObject.transform.position;
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
