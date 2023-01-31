using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//プレイヤー検知を行うクラス
public class Detection : MonoBehaviour
{
    private FlightlessMonster _flightless;
    public bool _moveLock;
    Monster _monster;

    // Start is called before the first frame update
    void Start()
    {
        _flightless = GetComponentInParent<FlightlessMonster>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _flightless._endScream && !_flightless._isMove)
        {
            Debug.Log("Player検知");
            //一番近いプレイヤーを目的地に
            _flightless._navMeshAgent.destination = FetchNearObjectWithTag("Player").transform.position;
        }
    }

    GameObject FetchNearObjectWithTag(string tagName)
    {
        // 該当タグが1つしか無い場合はそれを返す
        var targets = GameObject.FindGameObjectsWithTag(tagName);
        if (targets.Length == 1) return targets[0];

        GameObject result = null;
        var minTargetDistance = float.MaxValue;
        foreach (var target in targets)
        {
            // 前回計測したオブジェクトよりも近くにあれば記録
            var targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (!(targetDistance < minTargetDistance)) continue;
            minTargetDistance = targetDistance;
            result = target.transform.gameObject;
        }

        // 最後に記録されたオブジェクトを返す
        return result;
    }
}
