using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FlyingMonster : EnemyController, IMonsterDamageable
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
        Landing,
        Flying
    };

    private Animator _anim;
    private Monster _monster;
    public bool _endAttack;
    public bool _endScream;
    private bool _landing;     //地上にいるかどうか
    public NavMeshAgent _navMeshAgent;
    public bool _isMove;
    private float _sec;
    private int _Landing;
    //public SABoneCollider[] _cols;
    public EnemyState _enemyState;
    [SerializeField] private int ENEMY_HP = 100;
    [SerializeField] private int FLY_HEIGHT = 10;
    private float GAME_TIME;
    [SerializeField] private float TURN_STATE_TIME;
    private bool _isDead;
    private float MATERIAL_ALPHA = 1;
    public Slider _HPBar;
    public Slider _BulkHPBar;
    //public AttackState attackType;

    public struct AttackInfo
    {
        public AttackState _attackState;
        public int _damage;

        public AttackInfo(AttackState attackState,int damege)
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
        _enemyState = new EnemyState();
        _enemyState = EnemyState.Landing;
        //attackType = new AttackState();
        ENEMY_HP = 100;

        //各種フラグ
        _endAttack = false;
        _endScream = false;
        _isMove = true;
        _landing = true;
        _sec = 0;
        GAME_TIME = 0;
        TURN_STATE_TIME = 15;
        _Landing = 1;

        //技をセット
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new TailAttack();
        _monster.Waza4 = new FireBall();
        _monster.Waza5 = new FlyingFireBall();
    }

    private void Update()
    {
        if(_isDead)
        {
            return;
        }

        EnemyStateManager();
    }

    //地面についているときの処理
    void Landing()
    {
        Move();
        Attack();
    }

    //地上状態と飛行状態を管理するクラス
    void EnemyStateManager()
    {
        GAME_TIME += Time.deltaTime;

        //ここInvokeで行けそうじゃね？？
        if(GAME_TIME >= TURN_STATE_TIME)
        {
            _Landing *= -1;

            if(_Landing == 1)
            {
                _anim.SetTrigger("Land");
            }
            else if(_Landing == -1)
            {
                _anim.SetTrigger("TakeOff");
            }

            GAME_TIME = 0;
        }

        Debug.Log("ランディングの値は" + _Landing);

        if(_landing)
        {
            Landing();
        }
        else
        {
            Flying();
        }
    }

    #region 地上処理

    /// <summary>
    /// 移動処理　一番近いプレイヤーをdestinationとする
    /// </summary>
    public override void Move()
    {
        //ゲーム開始時吠え終わっていて
        if (_endScream)
        {
            Debug.Log("走っています");
            _anim.SetFloat("Run", _navMeshAgent.velocity.magnitude);

            if(_navMeshAgent.velocity.magnitude < 0.03f)
            {
                _isMove = false;
            }
            else
            {
                _isMove = true;
            }
        }
    }

    /// <summary>
    /// 攻撃処理 クールタイムが終わると同時に基本的にランダムに攻撃を繰り出す
    /// </summary>
    public override void Attack()
    {
        //ゲーム開始時3秒は攻撃を行わない->吠えた後すぐに攻撃してしまうのを防ぐため
        if(_endScream)
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
            switch (_attackInfo._attackState)
            {
                //ノーマルであればノーマルアタックを発動
                case AttackState.Attack1:
                    _monster.Attack(_monster.Waza2, _anim);
                    //モンスターの技情報を構造体に渡す
                    _attackInfo = new AttackInfo(AttackState.Attack1, _monster.Waza2._damage);
                    //クールタイム発生
                    CoolTimeManager(_monster.Waza2);
                    break;

                //爪であれば爪アタックを発動
                case AttackState.Attack2:
                    _monster.Attack(_monster.Waza3, _anim);
                    //モンスターの技情報を構造体に渡す
                    _attackInfo = new AttackInfo(AttackState.Attack2, _monster.Waza3._damage);
                    //クールタイム発生
                    CoolTimeManager(_monster.Waza3);
                    break;

                //角であれば角アタックを発動
                case AttackState.Attack3:
                    _monster.Attack(_monster.Waza4, _anim);
                    //モンスターの技情報を構造体に渡す
                    _attackInfo = new AttackInfo(AttackState.Attack3, _monster.Waza4._damage);
                    //クールタイム発生
                    CoolTimeManager(_monster.Waza4);
                    break;
            }
        }
    }

    #endregion

    #region 空中処理

    void Flying()
    {
        if(!_endAttack && _endScream)
        {
            _anim.SetTrigger("TakeOff");

            FlyingAttack();
        }
    }

    //飛行時の攻撃処理
    void FlyingAttack()
    {
        _monster.Attack(_monster.Waza5, _anim);
        CoolTimeManager(_monster.Waza5);
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

        if(GameSystem.DeathMonsterNum >= 4)
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

    public void  StartFlying()
    {
        _landing = false;
    }

    //飛行が終わったときにアニメーションイベントとして呼び出す
    public void EndFlying()
    {
        _landing = true;
    }

    #endregion
}

//============================================================================================================
// 技の種類
//============================================================================================================

/// <summary>
/// 技名：ノーマルアタック ダメージ：60 クールタイム：6秒
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
/// 技名：ノーマルアタック ダメージ：60 クールタイム：7秒
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
/// 技名：テイルアタック ダメージ：30 クールタイム：4秒
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