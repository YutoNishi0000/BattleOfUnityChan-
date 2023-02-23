using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SwordManagerScript : MonoBehaviour
{
    [SerializeField]
    public int _damage;     //ダメージ量
    [SerializeField] private ParticleSystem _swordEffect;
    bool _attackLock;
    private AudioSource _audioManager;
    [SerializeField] private AudioClip _audioClip;

    void Start()
    {
        _damage = 50;
        _attackLock = false;
        _audioManager = GetComponent<AudioSource>();
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

        //エフェクト生成
        Instantiate(_swordEffect, other.gameObject.transform.position, Quaternion.identity);

        bool counterAttack = GetComponentInParent<CharacterControlScript>()._counterCollider;

        int damage = counterAttack ? _damage * 3 : _damage;

        monsterDamageable.Damage(damage, counterAttack);
        Debug.Log("攻撃を与えました");

        monsterDamageable.ShakeUI();

        _audioManager.PlayOneShot(_audioClip);

        StartCoroutine(CoolTime());
    }

    IEnumerator CoolTime()
    {
        Debug.Log("クールタイム発生");
        _attackLock = true;
        yield return new WaitForSeconds(0.5f);
        _attackLock = false;
    }
}