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
    private bool _landing;     //�n��ɂ��邩�ǂ���
    private bool _isDead;
    private bool _onceScream;
    private bool _attackLock;     //�U���𐧌�����t���O
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

        //�e��t���O
        _endAttack = false;
        _endScream = false;
        _landing = true;
        _sec = 0;
        GAME_TIME = 0;
        TURN_STATE_TIME = 15;
        _Landing = 1;

        //�Z���Z�b�g
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new TailAttack();
        _monster.Waza4 = new FireBall();
        _monster.Waza5 = new FlyingFireBall();

        //���g�̃I�u�W�F�N�g���Z�b�g
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
            }
            else if (_Landing == -1)
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
            }
        }
        else
        {
            Flying();
            _navMeshAgent.destination = new Vector3(0, 1, 0);
            transform.DOLookAt(Instance.transform.position - transform.position, 0.1f);
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
                _attackLock = true;
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

    void AttackLock()
    {
        _attackLock = false;
    }

    #endregion

    #region �󒆏���

    void Flying()
    {
        if (!_endAttack && _endScream)
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
        //���i�����I����Ă���R�b��ɍU�����b�N����������
        Invoke(nameof(AttackLock), 3);
    }

    public void StartFlying()
    {
        _landing = false;
    }

    //��s���I������Ƃ��ɃA�j���[�V�����C�x���g�Ƃ��ČĂяo��
    public void EndFlying()
    {
        _landing = true;
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
