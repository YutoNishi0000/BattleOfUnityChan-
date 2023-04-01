using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ThirdMonster : EnemyController, IMonsterDamageable
{
    private Animator _anim;
    private Monster _monster;
    public NavMeshAgent _navMeshAgent;
    private GameObject _fireOutRegion;            //�����t�B�[���h�ɕ\��������
    private GameObject _fireInRegion;            //�����t�B�[���h�ɕ\��������
    private GameObject _effectPos;            //�����t�B�[���h�ɕ\��������
    private AudioSource _audioSource;
    public AttackInfo _attackInfo;
    public EnemyState _enemyState;
    private bool _endAttack;
    private bool _endScream;
    private int _Landing;
    private bool _landing;     //�n��ɂ��邩�ǂ���
    private float GAME_TIME;
    private bool _isDead;
    private bool _onceScream;
    private bool _attackLock;     //�U���𐧌�����t���O
    private bool _rollingAttack;   //��]�U���̃t���O
    [SerializeField] private int ENEMY_HP = 100;
    [SerializeField] private int FLY_HEIGHT = 10;
    [SerializeField] private float TURN_STATE_TIME;
    [SerializeField] private Slider _HPBar;
    [SerializeField] private Slider _BulkHPBar;
    [SerializeField] private AudioClip[] _audioClip;
    [SerializeField] private Material _nomalStateShader;
    [SerializeField] private Material _angryStateShader;
    [SerializeField] private Material _ground;   //�t�B�[���h�̃}�e���A��


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _monster = new Monster();

        //�e��t���O
        _endAttack = false;
        _endScream = false;
        _landing = true;
        GAME_TIME = 0;
        TURN_STATE_TIME = 15;
        _Landing = 1;
        _rollingAttack = false;
        _attackLock = true;

        //�Z���Z�b�g
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new TailAttack();
        _monster.Waza4 = new FireBall();
        _monster.Waza5 = new FlyingFireBall();

        _ground.SetFloat("_Side", 0);                 //�t�B�[���h�̏�Ԃ����Ƃɖ߂�
        _ground.SetFloat("_Blend", 0);                 //�A���t�@�l��0�Ɂi��\���j

        //���g�̃I�u�W�F�N�g���Z�b�g
        Instance.SetEnemyObject(this.gameObject);

        _effectPos = GameObject.Find("ThirdEffectPos");
        _fireOutRegion = _effectPos.transform.Find("FireResionOutSide").gameObject;
        _fireInRegion = _effectPos.transform.Find("FireResionInSide").gameObject;

        for (int i = 0; i < 2; i++)
        {
            _fireOutRegion.SetActive(false);
            _fireInRegion.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameSystem.Instance.GetGameState() != GameSystem.GameState.Battle)
        {
            _audioSource.Stop();
        }

        StateManager(ENEMY_HP);

        if(_isDead)
        {
            return;
        }

        //AdjustMonsterPos();

        EnemyStateManager();
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

        Debug.Log("�����f�B���O�̒l��" + _Landing);

        if (_landing)
        {
            Landing();

            //���������i�����I����Ă�����ړI�n���v���C���[�ɃZ�b�g����
            if (_endScream)
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
        else
        {
            Flying();
            _navMeshAgent.destination = new Vector3(0, 1, 0);
        }
    }

    #region �X�e�[�g�Ǘ�

    void StateManager(float EnemyHP)
    {
        if (EnemyHP > 300)
        {
            _enemyState = EnemyState.NOMAL_STATE;

            //�}�e���A���̓m�[�}��
            GetComponentInChildren<SkinnedMeshRenderer>().material = _nomalStateShader;
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
            _anim.SetFloat("Run", _navMeshAgent.velocity.magnitude);
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
        if(!_endAttack && _endScream)
        {
            FlyingAttack();
        }
        else if(_rollingAttack)
        {
            transform.Rotate(new Vector3(0, 200 * Time.deltaTime, 0));
        }
    }

    //��s���̍U������
    void FlyingAttack()
    {
        _monster.Attack(_monster.Waza5, _anim);
        CoolTimeManager(_monster.Waza5);
    }

    void UnLockFlyingAttack()
    {
        _endAttack = false;
    }

    #endregion

    #region ��e�֘A����

    public void Damage(int damage, bool counter)
    {
        if(!_endScream)
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

        if(GameSystem.DeathMonsterNum >= 4)
        {
            return;
        }

        MonsterGenerater.generater.GenerateMonster(GameSystem.DeathMonsterNum);
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    #endregion

    #region �A�j���[�V�����C�x���g

    public void StartScream()
    {
        _audioSource.PlayOneShot(_audioClip[0]);
    }

    //���i�����I������Ƃ��ɌĂяo�����
    public void EndScream()
    {
        _endScream = true;
    }

    public void  StartFlying()
    {
        _landing = false;
    }

    //��s���I������Ƃ��ɃA�j���[�V�����C�x���g�Ƃ��ČĂяo��
    public void EndFlying()
    {
        Debug.Log("�󒆏�ԉ���");
        _landing = true;
    }

    public void StartRollingAttack()
    {
        _rollingAttack = true;
        int rand = Random.Range(1, 3);
        _ground.SetFloat("_Side", rand);    //�����_���Ȓl�͂P���Q��I��
        _ground.SetFloat("_Blend", 0.3f);                 //�A���t�@�l��0.3��

        switch (rand)
        {
            case 1:
                _fireOutRegion.SetActive(true);
                break;
            case 2:
                _fireInRegion.SetActive(true);
                break;
        }
    }

    public void EndRollingAttack()
    {
        _rollingAttack = false;
        _ground.SetFloat("_Side", 0);                 //�t�B�[���h�̏�Ԃ����Ƃɖ߂�
        _ground.SetFloat("_Blend", 0);                 //�A���t�@�l��0�Ɂi��\���j

        for (int i = 0; i < 2; i++)
        {
            _fireOutRegion.SetActive(false);
            _fireInRegion.SetActive(false);
        }
    }

    #endregion

    #region �Q�b�^�[�A�Z�b�^�[

    public EnemyState GetEnemyState()
    {
        return _enemyState;
    }

    #endregion
}