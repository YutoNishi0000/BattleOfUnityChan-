using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Z���F�m�[�}���A�^�b�N �_���[�W�F30 �N�[���^�C���F3�b
/// </summary>
class Scream : Waza
{
    public Scream()
    {
        this.waza_name = "Scream";
        this._damage = 0;
        this._coolTime = 3;
    }
}

/// <summary>
/// �Z���F�m�[�}���A�^�b�N �_���[�W�F30 �N�[���^�C���F4�b
/// </summary>
class NomalAttack : Waza
{
    public NomalAttack()
    {
        this.waza_name = "NomalAttack";
        this._damage = 30;
        this._coolTime = 4;
    }
}

/// <summary>
/// �Z���F�܃A�^�b�N �_���[�W�F50 �N�[���^�C���F5�b
/// </summary>
class ClawAttack : Waza
{
    public ClawAttack()
    {
        this.waza_name = "ClawAttack";
        this._damage = 50;
        this._coolTime = 5;
    }
}

/// <summary>
/// �Z���F�p�A�^�b�N �_���[�W�F60 �N�[���^�C���F5�b
/// </summary>
class HornAttack : Waza
{
    public HornAttack()
    {
        this.waza_name = "HornAttack";
        this._damage = 60;
        this._coolTime = 5;
    }
}

/// <summary>
/// �Z���F�m�[�}���A�^�b�N �_���[�W�F60 �N�[���^�C���F6�b
/// </summary>
class FireBall : Waza
{
    public FireBall()
    {
        this.waza_name = "FireBall";
        this._damage = 60;
        this._coolTime = 6;
    }
}

/// <summary>
/// �Z���F�m�[�}���A�^�b�N �_���[�W�F60 �N�[���^�C���F7�b
/// </summary>
class FlyingFireBall : Waza
{
    public FlyingFireBall()
    {
        this.waza_name = "FlyingFireBall";
        this._damage = 70;
        this._coolTime = 6;
    }
}

/// <summary>
/// �Z���F�e�C���A�^�b�N �_���[�W�F30 �N�[���^�C���F4�b
/// </summary>
class TailAttack : Waza
{
    public TailAttack()
    {
        this.waza_name = "TailAttack";
        this._damage = 30;
        this._coolTime = 4;
    }
}