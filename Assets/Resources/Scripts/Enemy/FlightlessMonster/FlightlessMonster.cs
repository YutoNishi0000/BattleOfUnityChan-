using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//��ׂȂ��h���S���𐧌䂷��N���X
public class FlightlessMonster : EnemyController, IMonsterDamageable
{
    //�U�����@
    public enum AttackState
    {
        Attack1,      //�m�[�}��
        Attack2,       //��
        Attack3        //�p
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
    public bool _isMove;  //����Ȃ�
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
    private bool _attackLock;     //�U���𐧌�����t���O
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

        //�e��t���O
        _endAttack = false;
        _endScream = false;
        _isMove = true;
        _moveLock = false;
        _isDead = false;
        _attackLock = true;
        _onceScream = false;

        //�Z���Z�b�g
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new ClawAttack();
        _monster.Waza4 = new HornAttack();

        _monster.Attack(_monster.Waza1, _anim);

        for (int i = 0; i < _cols.Length; i++)
        {
            _cols[i].enabled = false;
            Debug.Log("�U���̓����蔻���������");
        }

        //���g�̃I�u�W�F�N�g���Z�b�g
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

    #region �ړ�����

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

    #endregion

    #region �U������

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
                //_anim.SetTrigger("ChangeState");
                _onceScream = true;
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

            Debug.Log("����");
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
        _audioSource.PlayOneShot(_audioClip[0]);
    }

    //���i�����I������Ƃ��ɌĂяo�����
    public void EndScream()
    {
        _endScream = true;
        //���i�����I����Ă���R�b��ɍU�����b�N����������
        Invoke(nameof(AttackLock), 3);
    }

    //��e���[�V�������I������Ƃ��ɌĂяo���֐�
    public void EndHit()
    {
        //GameObject.Destroy(gameObject.GetComponent<Animator>());
    }

    //�A�j���[�V�����C�x���g���ŃG�t�F�N�g���I���ɂ���ׂ̊֐�
    public void OnParticleSystem()
    {
        CreateParticleSystem2(_earthExplosion, _effectPos.position, transform.rotation, 3);
    }

    #endregion

    #region �Q�b�^�[�A�Z�b�^�[

    public EnemyState GetEnemyState()
    {
        return _enemyState;
    }

    #endregion

    #region ���̑�

    #endregion
}

//============================================================================================================
// �Z�̎��
//============================================================================================================

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