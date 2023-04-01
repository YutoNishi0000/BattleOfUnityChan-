using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class SecondMonster : EnemyController, IMonsterDamageable
{
    private Animator _anim;
    private Monster _monster;
    private NavMeshAgent _navMeshAgent;
    private AudioSource _audioSource;
    private EnemyState _enemyState;
    public AttackInfo _attackInfo;
    private bool _endAttack;
    private bool _endScream;
    private bool _landing;     //地上にいるかどうか
    private bool _isDead;
    private bool _onceScream;
    private bool _attackLock;     //攻撃を制限するフラグ
    private int _Landing;
    private float _sec;
    private float GAME_TIME;
    private float MATERIAL_ALPHA = 1;
    [SerializeField] private int ENEMY_HP = 100;
    [SerializeField] private int FLY_HEIGHT = 10;
    [SerializeField] private float TURN_STATE_TIME;
    [SerializeField] private Slider _HPBar;
    [SerializeField] private Slider _BulkHPBar;
    [SerializeField] private AudioClip[] _audioClip;
    [SerializeField] private Material _nomalStateShader;
    [SerializeField] private Material _angryStateShader;


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();

        _monster = new Monster();

        //各種フラグ
        _endAttack = false;
        _endScream = false;
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

        //自身のオブジェクトをセット
        Instance.SetEnemyObject(this.gameObject);
    }

    private void Update()
    {
        if (GameSystem.Instance.GetGameState() != GameSystem.GameState.Battle)
        {
            _audioSource.Stop();
        }

        StateManager(ENEMY_HP);

        if (_isDead)
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
        if (GAME_TIME >= TURN_STATE_TIME)
        {
            _Landing *= -1;

            if (_Landing == 1)
            {
                _anim.SetTrigger("Land");
            }
            else if (_Landing == -1)
            {
                _anim.SetTrigger("TakeOff");
            }

            GAME_TIME = 0;
        }

        Debug.Log("ランディングの値は" + _Landing);

        if (_landing)
        {
            Landing();

            //もしも遠吠えが終わっていたら目的地をプレイヤーにセットする
            if (_endScream)
            {
                _navMeshAgent.destination = Instance.gameObject.transform.position;
            }
        }
        else
        {
            Flying();
            _navMeshAgent.destination = new Vector3(0, 1, 0);
            transform.DOLookAt(Instance.transform.position - transform.position, 0.1f);
        }
    }

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
                _onceScream = true;
            }
        }
    }

    #endregion

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
        }
    }

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

    #region 空中処理

    void Flying()
    {
        if (!_endAttack && _endScream)
        {
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

        MonsterGenerater.generater.GenerateMonster(GameSystem.DeathMonsterNum);
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

    public void StartFlying()
    {
        _landing = false;
    }

    //飛行が終わったときにアニメーションイベントとして呼び出す
    public void EndFlying()
    {
        _landing = true;
    }

    public void OnParticleSystem()
    {

    }

    #endregion

    #region ゲッター、セッター

    public EnemyState GetEnemyState()
    {
        return _enemyState;
    }

    #endregion
}
