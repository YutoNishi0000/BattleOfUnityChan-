using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManagerScript : MonoBehaviour
{
    [SerializeField]
    public int _damage;     //ダメージ量
    bool _attackLock;

    // Start is called before the first frame update
    void Start()
    {
        _damage = 50;
        _attackLock = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_attackLock)
        {
            return;
        }

        IMonsterDamageable monsterDamageable = other.gameObject.GetComponentInParent<IMonsterDamageable>();

        if(monsterDamageable == null)
        {
            Debug.Log("インターフェイスを取得できませんでした");
            return;
        }

        Debug.Log("攻撃が当たりました");

        bool counterAttack = GetComponentInParent<CharacterControlScript>()._counterCollider;

        Debug.Log("カウンターフラグ：" + counterAttack); 

        int damage = counterAttack ? _damage * 3 : _damage;

        if(counterAttack)
        {
            Debug.Log("カウンターアタックが炸裂だぁぁぁぁl");
        }

        monsterDamageable.Damage(damage);
        Debug.Log("攻撃を与えました");

        StartCoroutine("CUnbeatableTime");
    }

    IEnumerator CUnbeatableTime()
    {
        _attackLock = true;
        yield return new WaitForSeconds(3);
        _attackLock = false;
    }
}