using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CharacterControlScript : MonoBehaviour, IDamage
{
    private Camera mainCam;

    //�ړ������ɕK�v�ȃR���|�[�l���g��ݒ�
    public Animator animator;                 //���[�V�������R���g���[�����邽��Animator���擾
    public CharacterController controller;    //�L�����N�^�[�ړ����Ǘ����邽��CharacterController���擾
    private PlayerAnimationEvents animEve;

    public Slider _bulkHPBar;
    public Slider _HPBar;

    //�ړ����x���̃p�����[�^�p�ϐ�(inspector�r���[�Őݒ�)
    public float speed;         //�L�����N�^�[�̈ړ����x
    public float jumpSpeed;     //�L�����N�^�[�̃W�����v��
    public float rotateSpeed;   //�L�����N�^�[�̕����]�����x
    public float gravity;       //�L�����ɂ�����d�͂̑傫��
    public GameObject _avoidancePos;
    private float _avoidSpeed = 10f;
    Vector3 InitialPos;
    Vector3 avoidDirection;

    Vector3 targetDirection;        //�ړ���������̃x�N�g��
    Vector3 moveDirection = Vector3.zero;

    //�퓬�p�ϐ�����ԃt���O�Ǘ�
    public GameObject Sword;                //���g�������Ă��錕
    bool MoveLock = false;                  //�ړ����b�N�t���O
    bool AttackLock = false;                //�A�˖h�~�p�U�����b�N�t���O
    bool invincible = false;                //���G�t���O
    bool Deadflag = false;                  //���S�t���O
    bool Gardflag = false;
    public bool _avoidance;/* = false*/                 //����t���O 
    public bool _counterAttack;
    public bool _counterCollider;
    bool _counterFlag;
    bool _finishLock;

    // Start is called before the first frame update
    void Start()
    {
        _counterAttack = false;
        _counterCollider = false;
        _counterFlag = false;
        _finishLock = false;

        //if (myPV.isMine)    //���L�����ł���Ύ��s
        {
            //MainCamera��target�ɂ��̃Q�[���I�u�W�F�N�g��ݒ�
            mainCam = Camera.main;
            mainCam.GetComponent<CameraScript>().target = this.gameObject.transform;
        }

        Sword.GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_finishLock)
        {
            return;
        }

        GardManager();

        //�ړ����b�NON�܂��͎��S�t���OON�ł���Έړ��A�U���������Ȃ�
        if (!MoveLock && !Deadflag && !Gardflag)
        {
            moveControl();  //�ړ��p�֐�
            RotationControl(); //����p�֐�
            if (_avoidance)
            {
                //AvoidanceManager();//���p�֐�
                Avoidance();
                DOTween.To(() => controller.height, (val) => { controller.height = val; }, 0, 0.3f);

                if (controller.velocity.magnitude <= 0)
                {
                    UnLockAvoidance();
                }
            }
            //�ŏI�I�Ȉړ�����
            //(���ꂪ������CharacterController�ɏ�񂪑����Ȃ����߁A�����Ȃ�)
            controller.Move(moveDirection * Time.deltaTime);
        }

        //�U�����b�N���������Ă��Ȃ���΍U���ł���
        if (!AttackLock)
        {
            //�U������
            AttackControl();
        }
    }

    void moveControl()
    {
        //���i�s�����v�Z
        //�L�[�{�[�h���͂��擾
        float v = Input.GetAxisRaw("Vertical");         //InputManager�́����̓���       
        float h = Input.GetAxisRaw("Horizontal");       //InputManager�́����̓��� 

        //�J�����̐��ʕ����x�N�g������Y�����������A���K�����ăL����������������擾
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right; //�J�����̉E�������擾

        //�J�����̕������l�������L�����̐i�s�������v�Z
        targetDirection = h * right + v * forward;

        //�n��ɂ���ꍇ�̏���
        if (controller.isGrounded)
        {
            //�ړ��̃x�N�g�����v�Z
            moveDirection = targetDirection * speed;

            //Jump�{�^���ŃW�����v����
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else        //�󒆑���̏����i�d�͉����x���j
        {
            float tempy = moveDirection.y;
            //(���̂Q���̏���������Ƌ󒆂ł����͕����ɓ�����悤�ɂȂ�)
            //moveDirection = Vector3.Scale(targetDirection, new Vector3(1, 0, 1)).normalized;
            //moveDirection *= speed;
            moveDirection.y = tempy - gravity * Time.deltaTime;
        }

        //���s�A�j���[�V�����Ǘ�
        if (v > .1 || v < -.1 || h > .1 || h < -.1) //(�ړ����͂������)
        {
            animator.SetFloat("Speed", 1f); //�L�������s�̃A�j���[�V�����J�n

            //����t���O���I�t�̏�Ԃő��s����E�{�^������������
            if(!_avoidance && Input.GetKeyDown(KeyCode.E))
            {
                _avoidance = true;
                avoidDirection = transform.forward;
            }
        }
        else    //(�ړ����͂�������)
        {
            animator.SetFloat("Speed", 0); //�L�������s�̃A�j���[�V�����I��
        }
    }

    //
    void GardManager()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !Gardflag)
        {
            //myPV.RPC("GardCounter", PhotonTargets.AllViaServer);
            GardCounter();
            _counterAttack = true;
            Debug.Log("�J�E���^�[�U���̂��߂̔���J�n");
        }
        else if (Input.GetKeyUp(KeyCode.Q) && Gardflag)
        {
            UnLockGard();
            CancelInvoke("UnLockGard");
        }
    }

    //�K�[�h->�^�C�~���O������΃J�E���^�[�U��
    //[PunRPC]
    void GardCounter()
    {
        Gardflag = true;
        animator.SetFloat("ResetGard", 0);
        animator.SetFloat("Gard", 1);
        Invoke(nameof(UnLockGard), 2);
    }

    void UnLockGard()
    {
        animator.SetFloat("ResetGard", 1);
        Gardflag = false;
    }

    //������s���֐������ۂɎ��s����֐�
    void AvoidanceManager()
    {
        //�������s��
        //myPV.RPC("Avoidance", PhotonTargets.AllViaServer);
    }

    //���(�����ɉ�����J��o�����߂ɒl�͂O��1�Œ���)�A���͊Ǘ��͈ړ������̒��ōs���A�����Ă���Ƃ��ɂ����J��o���Ȃ��A�{�^�����������玩���I�ɉ�����Ă����悤�Ȋ֐�������
    //����J�n���̃��[�V�����Ɖ���������̃��[�V�����𓯊������������߂����ň�ʂ�̉���������L�q���Ă���B
    //[PunRPC]
    void Avoidance()
    {
        //if (_avoidance)
        {
            animator.SetFloat("ResetAvoidance", 0);
            animator.SetFloat("Avoidance", 1);
        }

        //��b��ɉ������������
        Invoke(nameof(UnLockAvoidance), 0.6f);
    }

    void UnLockAvoidance()
    {
        //�������
        animator.SetFloat("ResetAvoidance", 1);
        _avoidance = false;
        //�����̓����_�����g���ĕϐ���ݒ�
        DOTween.To(() => controller.height, (val) => { controller.height = val; }, 2, 1f);
    }

    void RotationControl()  //�L�����N�^�[���ړ�������ς���Ƃ��̏���
    {
        Vector3 rotateDirection = moveDirection;
        rotateDirection.y = 0;

        //����Ȃ�Ɉړ��������ω�����ꍇ�݈̂ړ�������ς���
        if (rotateDirection.sqrMagnitude > 0.01)
        {
            //�ɂ₩�Ɉړ�������ς���
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.Slerp(transform.forward, rotateDirection, step);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    //�{�[���U��
    void AttackControl()
    {
        if (Input.GetMouseButtonDown(0) && !animator.IsInTransition(0))	//�@�J�ړr���łȂ�
        {
            //�U�����b�N�J�n
            AttackLock = true;
            //�ړ����b�N�J�n
            MoveLock = true;
            StartCoroutine(_ballattack(1f));
        }
    }

    IEnumerator _ballattack(float pausetime)
    {
        //RPC�Ń{�[������
        //myPV.RPC("BallInst", PhotonTargets.AllViaServer, transform.position + transform.up, transform.rotation);
        animator.SetTrigger("Attack");
        //�U���d���̂���pausetime�����҂�
        yield return new WaitForSeconds(pausetime);
        //�U�����b�N����
        AttackLock = false;
        //�ړ����b�N����
        MoveLock = false;
    }

    #region ��e�֘A����

    void OnTriggerEnter(Collider col)
    {
        if (Deadflag || invincible || _avoidance) //���S���܂��͖��G���A������͏������Ȃ�
        {
            return;
        }

        if(col.CompareTag("Ground"))
        {
            InitialPos = transform.position;
        }

        //�������������{�[���ł͂Ȃ��܂��͎������������Ă��錕�Ȃ�Ȃɂ����Ȃ��A�܂��v���C���[�ł����Ă��������Ȃ�
        if (col.CompareTag("Ball") || /*colAttacker.IsLocal || */col.CompareTag("Player") || col.CompareTag("Enemy") || col.CompareTag("Untagged"))
        {
            return;
        }
        else
        {
            ////�_���[�W��^����
            //LocalVariables.currentHP -= 10;

            if (col.CompareTag("AttackCollider"))
            {
                if(_counterAttack)
                {
                    animator.SetTrigger("CounterAttack");
                    _counterFlag = true;
                    return;
                }
            }
        }
    }

    //��e���������pRPC
    //[PunRPC]
    void Damaged()
    {
        MoveLock = true;    //�d���̂��߈ړ����b�NON
        animator.SetTrigger("DamagedTrigger");  //�_���[�W�A�j���[�V����
    }

    //�q�b�g���d������
    IEnumerator _rigor(float pausetime)
    {
        yield return new WaitForSeconds(pausetime); //�|��Ă��鎞��
        MoveLock = false;   //�ړ����b�N����
    }

    public void Damage(int damage)
    {
        LocalVariables.currentHP -= damage;
        _bulkHPBar.value = LocalVariables.currentHP;
        _HPBar.DOValue(LocalVariables.currentHP, 0.5f);

        if(LocalVariables.currentHP <= 0)
        {
            Dead();
        }
        else
        {
            Damaged();
            StartCoroutine(_rigor(.5f));    //��e�d������
        }
    }

    //���S���������pRPC
    void Dead()
    {
        if(Deadflag)
        {
            return;
        }

        Deadflag = true;    //���S�t���OON
        AttackLock = true;  //�U�����b�NON
        MoveLock = true;    //�ړ����b�NON
        animator.SetTrigger("DeathTrigger");    //���S�A�j���[�V����ON
    }

    //�����R���[�`��
    IEnumerator _revive(float pausetime)
    {
        yield return new WaitForSeconds(pausetime); //�|��Ă��鎞��
        //����
        Deadflag = false;   //���S����
        AttackLock = false; //�U�����b�N����
        MoveLock = false;   //�ړ����b�N����
        invincible = true;  //���S�㖳�G�J�n
        LocalVariables.currentHP = 100; //HP��
        yield return new WaitForSeconds(5f);    //���S�㖳�G����
        invincible = false; //���G����
    }
    #endregion

    #region �A�j���[�V�����C�x���g

    //�U���A�j���[�V�����J�n���ɌĂяo��
    public void StartAttack()
    {
        if(_counterFlag)
        {
            _counterCollider = true;
        }

        //���̓����蔻����I����
        Sword.GetComponent<Collider>().enabled = true;
    }

    //�U���A�j���[�V�����I�����ɌĂяo��
    public void EndAttack()
    {
        if(_counterCollider)
        {
            _counterCollider = false;
            _counterFlag = false;
        }

        //���̓����蔻����I�t��
        Sword.GetComponent<Collider>().enabled = false;
    }

    public void EndCounterAttack()
    {
        _counterAttack = false;
        Debug.Log("�J�E���^�[�U���̂��߂̔���I��");
    }

    public void OnCounterCollider()
    {
        _counterCollider = true;
        //���̓����蔻����I����
        Sword.GetComponent<Collider>().enabled = true;
    }

    public void OffCounterCollider()
    {
        _counterCollider = false;
        //���̓����蔻����I�t��
        Sword.GetComponent<Collider>().enabled = false;
    }

    #endregion

    #region �Q�[���N���A�A�Q�[���I�[�o�[����

    public void GameClear()
    {
        if(GameSystem.DeathMonsterNum == 4)
        {
            animator.SetTrigger("GAMECLEAR");
            _finishLock = true;
        }
    }

    public void GameOver()
    {
        if(LocalVariables.currentHP <= 0)
        {
            animator.SetTrigger("GAMEOVER");
            _finishLock = true;
        }
    }

    #endregion
}

//�_���[�W�����C���^�[�t�F�C�X
interface IDamage
{
    //�_���[�W����
    public void Damage(int damage);
}


//���N���X
public class Actor : MonoBehaviour
{
    protected CharacterController Instance;
    protected GameSystem gameSystem;

    private void Awake()
    {
        Instance = FindObjectOfType<CharacterController>();
        gameSystem = FindObjectOfType<GameSystem>();
    }
}