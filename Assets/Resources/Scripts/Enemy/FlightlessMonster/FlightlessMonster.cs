using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//飛べないドラゴンを制御するクラス
public class FlightlessMonster : EnemyController, IMonsterDamageable
{
    //攻撃方法
    public enum AttackState
    {
        Attack1,      //ノーマル
        Attack2,       //爪
        Attack3        //角
    }

    private Animator _anim;
    private Monster _monster;
    public bool _endAttack;
    public bool _endScream;
    public NavMeshAgent _navMeshAgent;
    public bool _isMove;
    public bool _moveLock;
    private float _sec;
    public SABoneCollider[] _cols;
    private bool _isDead;
    private float MATERIAL_ALPHA = 1;
    public Slider _HPBar;
    public Slider _BulkHPBar;
    public ParticleSystem _earthExplosion;
    public Transform _effectPos;

    [SerializeField] private int ENEMY_HP = 5000;

    public struct AttackInfo
    {
        public AttackState _attackState;
        public int _damage;

        public AttackInfo(AttackState attackState, int damege)
        {
            _attackState = attackState;
            _damage = damege;
        }
    }


    public AttackInfo _attackInfo;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _monster = new Monster();

        ENEMY_HP = 100;

        //各種フラグ
        _endAttack = false;
        _endScream = false;
        _isMove = true;
        _moveLock = false;
        _isDead = false;

        //技をセット
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new ClawAttack();
        _monster.Waza4 = new HornAttack();

        _monster.Attack(_monster.Waza1, _anim);

        for(int i = 0; i < _cols.Length; i++)
        {
            _cols[i].enabled = false;
            Debug.Log("攻撃の当たり判定を初期化");
        }

        //自身のオブジェクトをセット
        Instance.SetEnemyObject(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
        {
            return;
        }

        Attack();
        Move();
    }

    #region 移動処理

    /// <summary>
    /// 移動処理　一番近いプレイヤーをdestinationとする
    /// </summary>
    public override void Move()
    {
        //ゲーム開始時吠え終わっていて
        if (_endScream)
        {
            _anim.SetFloat("Run", _navMeshAgent.velocity.magnitude);

            if (_navMeshAgent.velocity.magnitude < 0.0001f)
            {
                _isMove = false;
            }
            else
            {
                _isMove = true;
            }
        }
    }

    #endregion

    #region 攻撃処理

    /// <summary>
    /// 攻撃処理 クールタイムが終わると同時に基本的にランダムに攻撃を繰り出す
    /// </summary>
    public override void Attack()
    {
        if (_endScream)
        {
            _sec += Time.deltaTime;

            if (_sec < 3)
            {
                return;
            }
            else
            {
                _sec = 3;
            }
        }

        //クールタイムが終わっていてゲーム開始時吠え終わっていてかつ移動していなければ
        if (!_endAttack && _endScream && !_isMove)
        {
            //ランダムに一つだけ攻撃の種類を取得
            _attackInfo._attackState = (AttackState)Random.Range(0, 3);

            //一つだけ取得した攻撃の種類が
            switch(_attackInfo._attackState)
            {
                //ノーマルであればノーマルアタックを発動
                case AttackState.Attack1:
                    _monster.Attack(_monster.Waza2, _anim);
                    //クールタイム発生
                    CoolTimeManager(_monster.Waza2);
                    _attackInfo._damage = _monster.Waza2._damage;
                    break;

                //爪であれば爪アタックを発動
                case AttackState.Attack2:
                    _monster.Attack(_monster.Waza3, _anim);
                    //クールタイム発生
                    CoolTimeManager(_monster.Waza3);
                    _attackInfo._damage = _monster.Waza3._damage;
                    break;

                //角であれば角アタックを発動
                case AttackState.Attack3:
                    _monster.Attack(_monster.Waza4, _anim);
                    //クールタイム発生
                    CoolTimeManager(_monster.Waza4);
                    _attackInfo._damage = _monster.Waza4._damage;
                    break;
            }
        }
    }

    #endregion

    #region 被弾関連処理

    public void Damage(int damage)
    {
        ENEMY_HP -= damage;
        _BulkHPBar.value = ENEMY_HP;
        _HPBar.DOValue(ENEMY_HP, 0.5f);
        if (ENEMY_HP > 0)
        {
            _anim.SetTrigger("Hit");
        }
        else
        {
            if (_isDead)
            {
                //既に死亡モーションが再生されていたら処理しない
                return;
            }

            _anim.SetTrigger("DeathTrigger");
            _isDead = true;
        }
    }

    #endregion

    #region クールタイム

    //クールタイムを管理するクラス
    void CoolTimeManager(Waza waza)
    {
        //コルーチン呼び出し
        StartCoroutine(CCoolTime(waza._coolTime));
    }

    //コルーチン
    IEnumerator CCoolTime(float coolTime)
    {
        _endAttack = true;
        yield return new WaitForSeconds(coolTime);
        _endAttack = false;
    }

    #endregion

    #region 死亡処理

    //死亡処理
    public override void Death()
    {
        GameSystem.DeathMonsterNum++;

        if (GameSystem.DeathMonsterNum >= 4)
        {
            return;
        }

        gameSystem.GenerateMonster(GameSystem.DeathMonsterNum);
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    #endregion

    #region アニメーションイベント

    //遠吠えが終わったときに呼び出される
    public void EndScream()
    {
        _endScream = true;
    }

    //被弾モーションが終わったときに呼び出す関数
    public void EndHit()
    {
        //GameObject.Destroy(gameObject.GetComponent<Animator>());
    }

    //アニメーションイベント内でエフェクトをオンにする為の関数
    public void OnParticleSystem()
    {
        CreateParticleSystem2(_earthExplosion, _effectPos.position, transform.rotation, 3);
    }

    #endregion
}

//============================================================================================================
// 技の種類
//============================================================================================================

/// <summary>
/// 技名：ノーマルアタック ダメージ：30 クールタイム：3秒
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
/// 技名：ノーマルアタック ダメージ：30 クールタイム：4秒
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
/// 技名：爪アタック ダメージ：50 クールタイム：5秒
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
/// 技名：角アタック ダメージ：60 クールタイム：5秒
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