using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����X�^�[�̊��N���X
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

//�_���[�W�����p�C���^�[�t�F�C�X
interface IMonsterDamageable
{
    public void Damage(int damage);
}


/// <summary>
/// �����X�^�[���N���X
/// </summary>
class Monster
{
    /// <summary>
    /// �����X�^�[�̋Z��� �o����Z�͂Q��
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


    //�����X�^�[�̍U��
    public void Attack(Waza _waza, Animator _anim)
    {
        _moveLock = true;
        _anim.SetTrigger(_waza.waza_name);
    }
}

//�Z�̒��ۃN���X
abstract class Waza
{
    /// <summary>
    /// �Z�̖��O
    /// </summary>
    public string waza_name;

    /// <summary>
    /// �Z�̈З�
    /// </summary>
    public int _damage;

    /// <summary>
    /// �Z���g������̃N�[���^�C��
    /// </summary>
    public float _coolTime;
}
