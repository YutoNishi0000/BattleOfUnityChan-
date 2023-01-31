using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//モンスターの基底クラス
public class EnemyController : Actor
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Move() { }

    public virtual void Attack() { }

    public virtual void GetHit() { }

    public virtual void Death() { }
}

//ダメージ処理用インターフェイス
interface IMonsterDamageable
{
    public void Damage(int damage);
}


/// <summary>
/// モンスター基底クラス
/// </summary>
class Monster
{
    /// <summary>
    /// モンスターの技情報 覚える技は２つ
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
