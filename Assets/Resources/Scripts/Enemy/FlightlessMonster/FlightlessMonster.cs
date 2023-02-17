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

        //�e��t���O
        _endAttack = false;
        _endScream = false;
        _isMove = true;
        _moveLock = false;
        _isDead = false;

        //�Z���Z�b�g
        _monster.Waza1 = new Scream();
        _monster.Waza2 = new NomalAttack();
        _monster.Waza3 = new ClawAttack();
        _monster.Waza4 = new HornAttack();

        _monster.Attack(_monster.Waza1, _anim);

        for(int i = 0; i < _cols.Length; i++)
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
        if (_isDead)
        {
            return;
        }

        Attack();
        Move();
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

    #region �U������

    /// <summary>
    /// �U������ �N�[���^�C�����I���Ɠ����Ɋ�{�I�Ƀ����_���ɍU�����J��o��
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

        //�N�[���^�C�����I����Ă��ăQ�[���J�n���i���I����Ă��Ă��ړ����Ă��Ȃ����
        if (!_endAttack && _endScream && !_isMove)
        {
            //�����_���Ɉ�����U���̎�ނ��擾
            _attackInfo._attackState = (AttackState)Random.Range(0, 3);

            //������擾�����U���̎�ނ�
            switch(_attackInfo._attackState)
            {
                //�m�[�}���ł���΃m�[�}���A�^�b�N�𔭓�
                case AttackState.Attack1:
                    _monster.Attack(_monster.Waza2, _anim);
                    //�N�[���^�C������
                    CoolTimeManager(_monster.Waza2);
                    _attackInfo._damage = _monster.Waza2._damage;
                    break;

                //�܂ł���Β܃A�^�b�N�𔭓�
                case AttackState.Attack2:
                    _monster.Attack(_monster.Waza3, _anim);
                    //�N�[���^�C������
                    CoolTimeManager(_monster.Waza3);
                    _attackInfo._damage = _monster.Waza3._damage;
                    break;

                //�p�ł���Ίp�A�^�b�N�𔭓�
                case AttackState.Attack3:
                    _monster.Attack(_monster.Waza4, _anim);
                    //�N�[���^�C������
                    CoolTimeManager(_monster.Waza4);
                    _attackInfo._damage = _monster.Waza4._damage;
                    break;
            }
        }
    }

    #endregion

    #region ��e�֘A����

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
                //���Ɏ��S���[�V�������Đ�����Ă����珈�����Ȃ�
                return;
            }

            _anim.SetTrigger("DeathTrigger");
            _isDead = true;
        }
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

    //���i�����I������Ƃ��ɌĂяo�����
    public void EndScream()
    {
        _endScream = true;
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