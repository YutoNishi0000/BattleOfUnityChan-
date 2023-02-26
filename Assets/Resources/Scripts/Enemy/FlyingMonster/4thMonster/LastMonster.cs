using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class LastMonster : EnemyController, IMonsterDamageable
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

    private Animator _anim;
    private Monster _monster;
    public bool _endAttack;
    public bool _endScream;
    public bool _landing;     //地上にいるかどうか
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
    public bool a;
    private bool _startLand;                      //着陸開始
    public bool _flyingMove;                      //空中移動を開始するかどうか
    private BlendSkybox _skyBox;
    [SerializeField] private Material _skyStateShader;
    //[SerializeField] private Material _defaultShader;
    private Vector3 _playerPos;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;
    [SerializeField] private Material _nomalStateShader;
    [SerializeField] private Material _angryStateShader;
    private bool _onceScream;
    private bool _attackLock;     //攻撃を制限するフラグ
    private GameObject flyingPos;

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


    // Start is called before the first frame update
    void Start()
    {
        _startLand = false;
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _skyBox = FindObjectOfType<BlendSkybox>();
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _nomalStateShader;
        _audioSource = GetComponent<AudioSource>();

        _monster = new Monster();
        //attackType = new AttackState();
        ENEMY_HP = 1000;

        //各種フラグ
        _endAttack = false;
        _endScream = false;
        _isMove = true;
        _landing = true;
        _sec = 0;
        GAME_TIME = 0;
        TURN_STATE_TIME = 15;
        _Landing = 1;
        _flyingMove = false;
        _attackLock = true;

        //技をセット
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new TailAttack();
        _monster.Waza4 = new FireBall();
        _monster.Waza5 = new FlyingFireBall();


        //自身のオブジェクトをセット
        Instance.SetEnemyObject(this.gameObject);
        flyingPos = GameObject.Find("flyingPosition");
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

        AdjustMonsterPos();


        ///ここでモンスターの向きの調整を行う！！！

        //地上にいて、吠え終わっていたら
        if (_landing && _endScream)
        {
            //プレイヤーの方向を向く
            transform.DOLookAt(Instance.gameObject.transform.position, 0.1f);
        }   
        //そうじゃなかったら
        else
        {
            //前を向く
            transform.rotation = Quaternion.identity;
        }
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
                _skyBox.InitializeSkybox();
                _playerPos = Instance.gameObject.transform.position;
                _startLand = true;
            }
            else if (_Landing == -1)
            {
                _anim.SetTrigger("TakeOff");
            }

            GAME_TIME = 0;
        }

        if(_startLand)
        {
            transform.DOMove(_playerPos, 2);
        }

        Debug.Log("ランディングの値は" + _Landing);

        if (_landing)
        {
            Landing();
        }
        else
        {
            Flying();
        }
    }

    #region ステート管理

    void StateManager(float EnemyHP)
    {
        if (EnemyHP > 300)
        {
            _enemyState = EnemyState.NOMAL_STATE;

            if (_landing)
            {
                //マテリアルはノーマル
                GetComponentInChildren<SkinnedMeshRenderer>().material = _nomalStateShader;
            }
            else
            {
                GetComponentInChildren<SkinnedMeshRenderer>().material = _skyStateShader;
            }
        }
        else
        {
            _enemyState = EnemyState.ANGRY_STATE;

            //ステートが変わったときにマテリアルを変更
            GetComponentInChildren<SkinnedMeshRenderer>().material = _angryStateShader;

            if (!_onceScream)
            {
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

            if (_navMeshAgent.velocity.magnitude < 0.03f)
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

    #endregion

    #region 空中処理

    void Flying()
    {
        if (!_endAttack && _endScream/* && !_attackLock*/ && !_startLand)
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

    //モンスターの移動位置に関する関数
    void AdjustMonsterPos()
    {
        if (_endScream && !_landing && _navMeshAgent.velocity.magnitude < 0.5f && !_startLand && _flyingMove)
        {
            _navMeshAgent.enabled = false;
            transform.DOMove(flyingPos.transform.position, 3);
        }
        else if (_endScream && _navMeshAgent.enabled && _landing)
        {
            _navMeshAgent.destination = Instance.gameObject.transform.position;

            float dis = Vector3.Distance(transform.position, Instance.transform.position);

            //モンスターとプレイヤーの距離が一定以上は慣れていたら攻撃できないようにする
            if (dis < _navMeshAgent.stoppingDistance)
            {
                _attackLock = false;
            }
            else
            {
                _attackLock = true;
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
            gameObject.SetActive(false);
            GameSystem.Instance.SetGameState(GameSystem.GameState.GameClear);
            return;
        }
    }

    #endregion

    #region アニメーションイベント

    public void StartScream()
    {
        GameSystem.Instance._shake.Shake(3f, 4f, 1);
        _audioSource.PlayOneShot(_audioClip[0]);
    }

    //遠吠えが終わったときに呼び出される
    public void EndScream()
    {
        _endScream = true;
    }

    public void StartFlying()
    {
        _landing = false;
        _skyBox.FadeSkybox();
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _skyStateShader;
    }

    public void StartMoveFlyPos()
    {
        _flyingMove = true;
    }

    public void StartLand()
    {
    }

    //飛行が終わったときにアニメーションイベントとして呼び出す
    public void EndFlying()
    {
        _landing = true;
        _navMeshAgent.enabled = true;
        _startLand = false;
        _flyingMove = false;
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
