using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//モンスターの基底クラス
public class EnemyController : Actor
{
    //攻撃方法
    public enum AttackState
    {
        Attack1,      //ノーマル
        Attack2,
        Attack3
    }

    public enum EnemyState
    {
        NOMAL_STATE,
        ANGRY_STATE
    }

    public struct AttackInfo
    {
        public AttackState _attackState;
        public float _damage;

        public AttackInfo(AttackState attackState, int damege)
        {
            _attackState = attackState;
            _damage = damege;
        }
    }

    public virtual void Move() { }

    public virtual void Attack() { }

    public virtual void GetHit() { }

    public virtual void Death() { }

    /// <summary>
    /// パーティクルシステムを生成するための関数
    /// </summary>
    /// <param name="particle">生成したいパーティクルシステム</param>
    /// <param name="pos">生成したい場所</param>
    /// <param name="rot">指定したい回転</param>
    /// <param name="destroyTime">何秒後にこのパーティクルシステムを消すか</param>
    public void CreateParticleSystem(ParticleSystem particle, Transform pos, Quaternion rot, float destroyTime)
    {
        //パーティクルシステム作成＆指定した場所の子オブジェクトにパーティクルシステムを設置
        ParticleSystem particleSystem = Instantiate(particle, pos);

        //回転を自身に合わせる
        particleSystem.transform.rotation = rot;

        //エフェクト再生
        particleSystem.Play();

        ////3秒後に消す
        //Destroy(particleSystem, destroyTime);
    }

    public void CreateParticleSystem2(ParticleSystem particle, Vector3 pos, Quaternion rot, float destroyTime)
    {
        //パーティクルシステム作成＆指定した場所の子オブジェクトにパーティクルシステムを設置
        ParticleSystem particleSystem = Instantiate(particle, pos, rot);

        //エフェクト再生
        particleSystem.Play();

        ////3秒後に消す
        //Destroy(particleSystem, destroyTime);
    }

    void StopParticleSystem(ParticleSystem particle)
    {
        StartCoroutine(nameof(CStopEffects), 3);
        particle.Stop();
        Destroy(particle);
    }

    IEnumerator CStopEffects(float time)
    {
        yield return new WaitForSeconds(time);
    }
}

//ダメージ処理用インターフェイス
interface IMonsterDamageable
{
    public void Damage(int damage, bool counter);

    public void ShakeUI();
}

/// <summary>
/// モンスター基底クラス
/// </summary>
class Monster
{
    /// <summary>
    /// モンスターの技情報 覚える技は６つ
    /// </summary>
    private Waza _waza1;
    private Waza _waza2;
    private Waza _waza3;
    private Waza _waza4;
    private Waza _waza5;
    private Waza _waza6;
    public bool _moveLock;

    //Property
    public Waza Waza1
    {
        get { return this._waza1; }
        set { this._waza1 = value; }
    }
    //Property
    public Waza Waza2
    {
        get { return this._waza2; }
        set { this._waza2 = value; }
    }
    //Property
    public Waza Waza3
    {
        get { return this._waza3; }
        set { this._waza3 = value; }
    }
    //Property
    public Waza Waza4
    {
        get { return this._waza4; }
        set { this._waza4 = value; }
    }

    //Property
    public Waza Waza5
    {
        get { return this._waza5; }
        set { this._waza5 = value; }
    }

    //Property
    public Waza Waza6
    {
        get { return this._waza6; }
        set { this._waza6 = value; }
    }


    //モンスターの攻撃
    public void Attack(Waza _waza, Animator _anim)
    {
        _moveLock = true;
        _anim.SetTrigger(_waza.waza_name);
    }
}

//技の抽象クラス
abstract class Waza
{
    /// <summary>
    /// 技の名前
    /// </summary>
    public string waza_name;

    /// <summary>
    /// 技の威力
    /// </summary>
    public int _damage;

    /// <summary>
    /// 技を使った後のクールタイム
    /// </summary>
    public float _coolTime;
}
