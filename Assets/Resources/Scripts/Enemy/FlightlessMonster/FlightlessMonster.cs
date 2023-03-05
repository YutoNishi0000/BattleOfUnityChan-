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

    public enum EnemyState
    {
        NOMAL_STATE,
        ANGRY_STATE
    }

    private Animator _anim;
    private Monster _monster;
    public bool _endAttack;
    public bool _endScream;
    public NavMeshAgent _navMeshAgent;
    public bool _isMove;  //いらない
    public bool _moveLock;
    private float _sec;
    public SABoneCollider[] _cols;
    private bool _isDead;
    private float MATERIAL_ALPHA = 1;
    public Slider _HPBar;
    public Slider _BulkHPBar;
    public ParticleSystem _earthExplosion;
    public Transform _effectPos;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;
    private bool _attackLock;     //攻撃を制限するフラグ
    [SerializeField] private Material _nomalStateShader;
    [SerializeField] private Material _angryStateShader;

    [SerializeField] private float ENEMY_HP = 5000;
    private bool _onceScream;

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


    public AttackInfo _attackInfo;
    public EnemyState _enemyState;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();

        _monster = new Monster();
        _enemyState = new EnemyState();

        ENEMY_HP = 1000;

        //各種フラグ
        _endAttack = false;
        _endScream = false;
        _isMove = true;
        _moveLock = false;
        _isDead = false;
        _attackLock = true;
        _onceScream = false;

        //技をセット
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new ClawAttack();
        _monster.Waza4 = new HornAttack();

        _monster.Attack(_monster.Waza1, _anim);

        for (int i = 0; i < _cols.Length; i++)
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
        if(GameSystem.Instance.GetGameState() != GameSystem.GameState.Battle)
        {
            _audioSource.Stop();
        }

        StateManager(ENEMY_HP);

        if (_isDead)
        {
            return;
        }

        Attack();
        Move();

        if (_endScream)
        {
            _navMeshAgent.destination = Instance.gameObject.transform.position;
        }
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
        }
    }

    #endregion

    #region 攻撃処理

    /// <summary>
    /// 攻撃処理 クールタイムが終わると同時に基本的にランダムに攻撃を繰り出す
    /// </summary>
    public override void Attack()
    {
        //クールタイムが終わっていてゲーム開始時吠え終わっていてかつ移動していなければ
        if (!_endAttack && _endScream && !_attackLock)
        {
            switch (GetEnemyState())
            {
                case EnemyState.NOMAL_STATE:
                    //攻撃方法はノーマル、角アタックのみでダメージ倍率１倍
                    State_Attack((AttackState)Random.Range(0, 2), 1);
                    break;

                case EnemyState.ANGRY_STATE:
                    //攻撃方法はノーマル、角、爪アタックでダメージ倍率２倍
                    State_Attack((AttackState)Random.Range(0, 3), 2);
                    break;
            }
        }
    }

    void State_Attack(AttackState attackState, float damageMultiplier)
    {
        //ランダムに一つだけ攻撃の種類を取得
        _attackInfo._attackState = attackState;

        //一つだけ取得した攻撃の種類が
        switch (_attackInfo._attackState)
        {
            //ノーマルであればノーマルアタックを発動
            case AttackState.Attack1:
                _monster.Attack(_monster.Waza2, _anim);
                //クールタイム発生
                CoolTimeManager(_monster.Waza2);
                _attackInfo._damage = _monster.Waza2._damage * damageMultiplier;
                break;

            //爪であれば角アタックを発動
            case AttackState.Attack2:
                _monster.Attack(_monster.Waza4, _anim);
                //クールタイム発生
                CoolTimeManager(_monster.Waza4);
                _attackInfo._damage = _monster.Waza4._damage * damageMultiplier;
                break;

            //角であれば爪アタックを発動
            case AttackState.Attack3:
                _monster.Attack(_monster.Waza3, _anim);
                //クールタイム発生
                CoolTimeManager(_monster.Waza3);
                _attackInfo._damage = _monster.Waza3._damage * damageMultiplier;
                break;
        }
    }

    void AttackLock()
    {
        _attackLock = false;
    }

    #endregion

    #region ステート管理

    void StateManager(float EnemyHP)
    {
        if (EnemyHP > 300)
        {
            _enemyState = EnemyState.NOMAL_STATE;

            //マテリアルはノーマル
            GetComponentInChildren<SkinnedMeshRenderer>().material = _nomalStateShader;
        }
        else
        {
            _enemyState = EnemyState.ANGRY_STATE;

            //ステートが変わったときにマテリアルを変更
            GetComponentInChildren<SkinnedMeshRenderer>().material = _angryStateShader;

            if (!_onceScream)
            {
                _attackLock = true;
                _anim.Play("Scream");
                //_anim.SetTrigger("ChangeState");
                _onceScream = true;
            }
        }
    }

    #endregion

    #region 被弾関連処理

    public void Damage(int damage, bool counter)
    {
        if (!_endScream)
        {
            return;
        }

        ENEMY_HP -= damage;
        _BulkHPBar.value = ENEMY_HP;
        _HPBar.DOValue(ENEMY_HP, 0.5f);
        if (ENEMY_HP > 0)
        {
            if (counter)
            {
                _anim.SetTrigger("Hit");
            }
        }
        else
        {
            if (_isDead)
            {
                //既に死亡モーションが再生されていたら処理しない
                return;
            }

            Debug.Log("死んだ");
            _anim.SetTrigger("DeathTrigger");
            _audioSource.PlayOneShot(_audioClip[1]);
            _isDead = true;
        }
    }

    public void ShakeUI()
    {
        GetComponent<PerlinNoiseController>().StartShake(0.3f, 100, 10);
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

    public void StartScream()
    {
        _audioSource.PlayOneShot(_audioClip[0]);
    }

    //遠吠えが終わったときに呼び出される
    public void EndScream()
    {
        _endScream = true;
        //遠吠えが終わってから３秒後に攻撃ロックを解除する
        Invoke(nameof(AttackLock), 3);
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

    #region ゲッター、セッター

    public EnemyState GetEnemyState()
    {
        return _enemyState;
    }

    #endregion

    #region その他

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