using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class LastMonster : EnemyController, IMonsterDamageable
{
    //�U�����@
    public enum AttackState
    {
        Attack1,      //�m�[�}��
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
    public bool _landing;     //�n��ɂ��邩�ǂ���
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
    private bool _startLand;                      //�����J�n
    public bool _flyingMove;                      //�󒆈ړ����J�n���邩�ǂ���
    private BlendSkybox _skyBox;
    [SerializeField] private Material _skyStateShader;
    //[SerializeField] private Material _defaultShader;
    private Vector3 _playerPos;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;
    [SerializeField] private Material _nomalStateShader;
    [SerializeField] private Material _angryStateShader;
    private bool _onceScream;
    private bool _attackLock;     //�U���𐧌�����t���O
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

        //�e��t���O
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

        //�Z���Z�b�g
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new TailAttack();
        _monster.Waza4 = new FireBall();
        _monster.Waza5 = new FlyingFireBall();


        //���g�̃I�u�W�F�N�g���Z�b�g
        Instance.SetEnemyObject(this.gameObject);
        flyingPos = GameObject.Find("flyingPosition");
    }

    private void Update()
    {
        StateManager(ENEMY_HP);

        if (_isDead)
        {
            return;
        }

        EnemyStateManager();

        AdjustMonsterPos();


        ///�����Ń����X�^�[�̌����̒������s���I�I�I

        //�n��ɂ��āA�i���I����Ă�����
        if (_landing && _endScream)
        {
            //�v���C���[�̕���������
            transform.DOLookAt(Instance.gameObject.transform.position, 0.1f);
        }   
        //��������Ȃ�������
        else
        {
            //�O������
            transform.rotation = Quaternion.identity;
        }
    }

    //�n�ʂɂ��Ă���Ƃ��̏���
    void Landing()
    {
        Move();
        Attack();
    }

    //�n���ԂƔ�s��Ԃ��Ǘ�����N���X
    void EnemyStateManager()
    {
        GAME_TIME += Time.deltaTime;

        //����Invoke�ōs����������ˁH�H
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

        Debug.Log("�����f�B���O�̒l��" + _Landing);

        if (_landing)
        {
            Landing();
        }
        else
        {
            Flying();
        }
    }

    #region �X�e�[�g�Ǘ�

    void StateManager(float EnemyHP)
    {
        if (EnemyHP > 300)
        {
            _enemyState = EnemyState.NOMAL_STATE;

            if (_landing)
            {
                //�}�e���A���̓m�[�}��
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

            //�X�e�[�g���ς�����Ƃ��Ƀ}�e���A����ύX
            GetComponentInChildren<SkinnedMeshRenderer>().material = _angryStateShader;

            if (!_onceScream)
            {
                _anim.Play("Scream");
                _onceScream = true;
            }
        }
    }

    #endregion

    #region �n�㏈��

    /// <summary>
    /// �ړ������@��ԋ߂��v���C���[��destination�Ƃ���
    /// </summary>
    public override void Move()
    {
        //�Q�[���J�n���i���I����Ă���
        if (_endScream)
        {
            Debug.Log("�����Ă��܂�");
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
    /// �U������ �N�[���^�C�����I���Ɠ����Ɋ�{�I�Ƀ����_���ɍU�����J��o��
    /// </summary>
    public override void Attack()
    {
        //�N�[���^�C�����I����Ă��ăQ�[���J�n���i���I����Ă��Ă��ړ����Ă��Ȃ����
        if (!_endAttack && _endScream && !_attackLock)
        {
            switch (GetEnemyState())
            {
                case EnemyState.NOMAL_STATE:
                    //�U�����@�̓m�[�}���A�p�A�^�b�N�݂̂Ń_���[�W�{���P�{
                    State_Attack((AttackState)Random.Range(0, 2), 1);
                    break;

                case EnemyState.ANGRY_STATE:
                    //�U�����@�̓m�[�}���A�p�A�܃A�^�b�N�Ń_���[�W�{���Q�{
                    State_Attack((AttackState)Random.Range(0, 3), 2);
                    break;
            }
        }
    }

    void State_Attack(AttackState attackState, float damageMultiplier)
    {
        //�����_���Ɉ�����U���̎�ނ��擾
        _attackInfo._attackState = attackState;

        //������擾�����U���̎�ނ�
        switch (_attackInfo._attackState)
        {
            //�m�[�}���ł���΃m�[�}���A�^�b�N�𔭓�
            case AttackState.Attack1:
                _monster.Attack(_monster.Waza2, _anim);
                //�N�[���^�C������
                CoolTimeManager(_monster.Waza2);
                _attackInfo._damage = _monster.Waza2._damage * damageMultiplier;
                break;

            //�܂ł���Ίp�A�^�b�N�𔭓�
            case AttackState.Attack2:
                _monster.Attack(_monster.Waza4, _anim);
                //�N�[���^�C������
                CoolTimeManager(_monster.Waza4);
                _attackInfo._damage = _monster.Waza4._damage * damageMultiplier;
                break;

            //�p�ł���Β܃A�^�b�N�𔭓�
            case AttackState.Attack3:
                _monster.Attack(_monster.Waza3, _anim);
                //�N�[���^�C������
                CoolTimeManager(_monster.Waza3);
                _attackInfo._damage = _monster.Waza3._damage * damageMultiplier;
                break;
        }
    }

    #endregion

    #region �󒆏���

    void Flying()
    {
        if (!_endAttack && _endScream/* && !_attackLock*/ && !_startLand)
        {
            FlyingAttack();
        }
    }

    //��s���̍U������
    void FlyingAttack()
    {
        _monster.Attack(_monster.Waza5, _anim);
        CoolTimeManager(_monster.Waza5);
    }

    //�����X�^�[�̈ړ��ʒu�Ɋւ���֐�
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

            //�����X�^�[�ƃv���C���[�̋��������ȏ�͊���Ă�����U���ł��Ȃ��悤�ɂ���
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

    #region ��e�֘A����

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
                //���Ɏ��S���[�V�������Đ�����Ă����珈�����Ȃ�
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

    #region �N�[���^�C��

    //�N�[���^�C�����Ǘ�����N���X
    void CoolTimeManager(Waza waza)
    {
        //�R���[�`���Ăяo��
        StartCoroutine(CCoolTime(waza._coolTime));
    }

    //�R���[�`��
    IEnumerator CCoolTime(float coolTime)
    {
        _endAttack = true;
        yield return new WaitForSeconds(coolTime);
        _endAttack = false;
    }

    #endregion

    #region ���S����

    //���S����
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

    #region �A�j���[�V�����C�x���g

    public void StartScream()
    {
        GameSystem.Instance._shake.Shake(3f, 4f, 1);
        _audioSource.PlayOneShot(_audioClip[0]);
    }

    //���i�����I������Ƃ��ɌĂяo�����
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

    //��s���I������Ƃ��ɃA�j���[�V�����C�x���g�Ƃ��ČĂяo��
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

    #region �Q�b�^�[�A�Z�b�^�[

    public EnemyState GetEnemyState()
    {
        return _enemyState;
    }

    #endregion
}