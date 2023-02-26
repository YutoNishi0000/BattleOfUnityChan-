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
    private PlayerSEController audioManager;

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
    GameObject enemyObj;

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
    //bool _counterFlag;
    bool _finishLock;
    bool _gardflag;
    bool _damageReaction;
    // ��ʂ�Ԃɂ��邽�߂̃C���[�W
    public Image img;
    [SerializeField] private GameObject _swordMiracle;   //���̋O�ՃG�t�F�N�g



    //�@IK�Ŋp�x��L���ɂ��邩�ǂ���
    [SerializeField]
    private bool useIKRot = true;
    //�@�n�ʂƂ��郌�C���[
    [SerializeField]
    private LayerMask fieldLayer;
    //�@�E���̃E�G�C�g
    private float rightFootWeight = 0f;
    //�@�����̃E�G�C�g
    private float leftFootWeight = 0f;
    //�@�E���̈ʒu
    private Vector3 rightFootIKPosition;
    //�@�����̈ʒu
    private Vector3 leftFootIKPosition;
    //�@�E���̊p�x
    private Quaternion rightFootRot;
    //�@�����̊p�x
    private Quaternion leftFootRot;
    //�@�E���ƍ����̋���
    private float distance;
    //�@����t���ʒu�̃I�t�Z�b�g�l
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.06f, 0f);
    //�@�R���C�_�̒��S�ʒu
    private Vector3 defaultCenter;
    //�@���C���΂�����
    [SerializeField]
    private float rayRange = 1f;

    //�@�̂̏d�S�𒲐����鎞�̃X�s�[�h
    [SerializeField]
    private float bodyPositionSpeed = 50f;

    //�@���C���΂��ʒu�̒����l
    [SerializeField]
    private Vector3 rayPositionOffset = Vector3.up * 0.3f;

    //�@�̂̏d�S��ύX���邩�ǂ���
    [SerializeField]
    private bool isChangeBodyPosition = true;
    //�@�O��̑̂̏d�S�ʒu
    private Vector3 preBodyPosition;
    //�@���̃��C���n�ʂɂ��Ă��邩�ǂ���
    private bool rightFootGrounded;
    private bool leftFootGrounded;


    private float x;
    private float z;
    public float Speed = 1.0f;
    float smooth = 10f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        _counterAttack = false;
        _counterCollider = false;
        //_counterFlag = false;
        _finishLock = false;
        _gardflag = false;
        _damageReaction = false;

        Sword.GetComponent<Collider>().enabled = false;
        rb = GetComponent<Rigidbody>();
        audioManager = GetComponent<PlayerSEController>();
        img.color = Color.clear;
        _swordMiracle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(_finishLock || Deadflag)
        {
            return;
        }



        GardManager();

        img.color = Color.Lerp(this.img.color, Color.clear, Time.deltaTime);

        //�ړ����b�NON�܂��͎��S�t���OON�ł���Έړ��A�U���������Ȃ�
        if (!MoveLock && !Deadflag && !Gardflag)
        {
            moveControl();  //�ړ��p�֐�
            RotationControl(); //����p�֐�
        }

        //�U�����b�N���������Ă��Ȃ���΍U���ł���
        if (!AttackLock)
        {
            //�U������
            AttackControl();
        }
    }

    #region �ړ��A��]����

    void moveControl()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Avoidance();
        }
        if(_avoidance)
        {
            animator.SetBool("Avoid", true);
            rb.AddForce(avoidDirection / 3, ForceMode.Impulse);
            transform.rotation = Quaternion.LookRotation(avoidDirection);
            return;
        }

        //�i�s�����v�Z
        //�L�[�{�[�h���͂��擾
        float v = Input.GetAxisRaw("Vertical");         //InputManager�́����̓���       
        float h = Input.GetAxisRaw("Horizontal");       //InputManager�́����̓��� 

        //�J�����̐��ʕ����x�N�g������Y�����������A���K�����ăL����������������擾
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right; //�J�����̉E�������擾

        //�J�����̕������l�������L�����̐i�s�������v�Z
        targetDirection = h * right + v * forward;

        rb.velocity = targetDirection * GetDashSpeed(speed);

        //���s�A�j���[�V�����Ǘ�
        if (rb.velocity.magnitude > 0) //(�ړ����͂������)
        {
            animator.SetFloat("Speed", 1f); //�L�������s�̃A�j���[�V�����J�n
        }
        else    //(�ړ����͂�������)
        {
            animator.SetFloat("Speed", 0); //�L�������s�̃A�j���[�V�����I��
        }

    }

    /// <summary>
    /// �v���C���[�̉�]����
    /// </summary>
    void RotationControl()  //�L�����N�^�[���ړ�������ς���Ƃ��̏���
    {
        Vector3 rotateDirection = targetDirection;
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

    float GetDashSpeed(float speed)
    {
        //�������V�t�g���������A�܂��̓X���C�f�B���O��𒆂ł����
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //���i�̃X�s�[�h��1.5�{�̃X�s�[�h�̒l��Ԃ�
            return speed * 1.5f;
        }
        else
        {
            //���Ƀ_�b�V���������s���Ă��Ȃ����
            return speed;
        }
    }

    #endregion

    #region �K�[�h����

    //
    void GardManager()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !Gardflag)
        {
            GardCounter();
            _counterAttack = true;
            audioManager.Play("Gard");

            //0.5�b��ɃJ�E���^�[�t���O���I�t��
            Invoke(nameof(EndCounterAttack), 3f);
            Debug.Log("�J�E���^�[�U���̂��߂̔���J�n");
        }
        else if (Input.GetKeyUp(KeyCode.Q) && Gardflag)
        {
            UnLockGard();
            CancelInvoke("UnLockGard");
        }
    }

    //�K�[�h->�^�C�~���O������΃J�E���^�[�U��
    void GardCounter()
    {
        //�t���O���I����
        Gardflag = true;

        //�e��A�j���[�V�����Đ�
        animator.SetFloat("ResetGard", 0);
        animator.SetFloat("Gard", 1);

        //�K�[�h�I���
        Invoke(nameof(UnLockGard), 2);
    }

    void UnLockGard()
    {
        animator.SetFloat("ResetGard", 1);

        //�t���O���I�t��
        Gardflag = false;
    }

    #endregion

    #region �������

    //���(�����ɉ�����J��o�����߂ɒl�͂O��1�Œ���)�A���͊Ǘ��͈ړ������̒��ōs���A�����Ă���Ƃ��ɂ����J��o���Ȃ��A�{�^�����������玩���I�ɉ�����Ă����悤�Ȋ֐�������
    void Avoidance()
    {
        avoidDirection = transform.forward;
        audioManager.Play("Avoidance");
        StartCoroutine(CAvoidance(0.5f));
    }

    IEnumerator CAvoidance(float time)
    {
        _avoidance = true;
        yield return new WaitForSeconds(time);
        _avoidance = false;
        animator.SetBool("Avoid", false);
    }

    //������I������Ƃ��ɉ���Ɏg�����l�Ȃǂ̏������������s��
    public void UnLockAvoidance()
    {
        animator.ResetTrigger("Avoidance");
    }

    #endregion

    #region �U������

    void AttackControl()
    {
        if (Input.GetMouseButtonDown(0) && !animator.IsInTransition(0))	//�@�J�ړr���łȂ�
        {
            //�U�����b�N�J�n
            AttackLock = true;
            //�ړ����b�N�J�n
            MoveLock = true;
            StartCoroutine(CAttack(1f));
            AttackSE();
        }
    }

    IEnumerator CAttack(float pausetime)
    {
        animator.SetTrigger("Attack");
        //�U���d���̂���pausetime�����҂�
        yield return new WaitForSeconds(pausetime);
        //�U�����b�N����
        AttackLock = false;
        //�ړ����b�N����
        MoveLock = false;
    }

    #endregion

    #region IK�֘A

    /// <summary>
    /// ���IK�������X�^�[�̕����Ɍ����悤�ɕύX����
    /// </summary>
    public void OnAnimatorIK(int layerIndex)
    {
        if(_avoidance)
        {
            return;
        }

        //�@�A�j���[�V�����p�����[�^����IK�̃E�G�C�g���擾
        rightFootWeight = animator.GetFloat("RightFootWeight");
        leftFootWeight = animator.GetFloat("LeftFootWeight");

        //�@�E���p�̃��C�̎��o��
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot) + rayPositionOffset, Vector3.down * rayRange, Color.red);
        //�@�E���p�̃��C���΂�����
        var ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + rayPositionOffset, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayRange, fieldLayer))
        {
            rightFootGrounded = true;
            rightFootIKPosition = hit.point;

            //�@�E��IK�̐ݒ�
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKPosition + offset);
            if (useIKRot)
            {
                rightFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
            }
        }
        else
        {
            rightFootGrounded = false;
        }

        //�@�����p�̃��C���΂�����
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + rayPositionOffset, Vector3.down);
        //�@�����p�̃��C�̎��o��
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + rayPositionOffset, Vector3.down * rayRange, Color.red);

        if (Physics.Raycast(ray, out hit, rayRange, fieldLayer))
        {
            leftFootGrounded = true;
            leftFootIKPosition = hit.point;

            //�@����IK�̐ݒ�
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKPosition + offset);

            if (useIKRot)
            {
                leftFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRot);
            }
        }
        else
        {
            leftFootGrounded = false;
        }
        //�@�̂̏d�S�𓮂����ꍇ
        if (isChangeBodyPosition && rightFootGrounded && leftFootGrounded)
        {
            //�@���E�̑��ƃL�����N�^�[�̑����̈ʒu�Ƃ̋������v�Z
            var rightFootDistance = rightFootIKPosition.y - transform.position.y;
            var leftFootDistance = leftFootIKPosition.y - transform.position.y;
            //�@���E�̑��̈ʒu����艺�ɂ�����������Ƃ��Ďg��
            var distance = rightFootDistance < leftFootDistance ? rightFootDistance : leftFootDistance;
            //�@�̂̏d�S�����ɂ�����̑��ɍ��킹�ĉ�����
            var nowBodyPosition = animator.bodyPosition + Vector3.up * distance;
            //�@���X�ɕύX����悤�ɂ��Ă��邪�A���Ԃ�R�����g�ɂ��Ă��鏈���̂悤�Ɉ�C�ɕς��Ă����Ȃ�
            animator.bodyPosition = Vector3.Lerp(preBodyPosition, nowBodyPosition, bodyPositionSpeed * Time.deltaTime);
            preBodyPosition = animator.bodyPosition;
            //animator.bodyPosition = nowBodyPosition;

        }



        //=====================================================================================================================
        // ��������͊��IK��ύX���鏈���Ɉڂ�
        //=====================================================================================================================
        Vector3 target_pos = new Vector3(enemyObj.transform.position.x, 0, enemyObj.transform.position.z);
        Vector3 char_pos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = target_pos - char_pos;

        //���������Ń����X�^�[�����̃x�N�g���ƁA�����������Ă�������̃x�N�g���Ƃ̊p�x���擾
        float angle = Vector3.Angle(direction, moveDirection);

        //���ŋ��߂��p�x��150�x�ȏ�ł����
        if (angle >= 150)
        {
            //�������炵���̏����͍s��Ȃ�
            return;
        }

        animator.SetLookAtPosition(enemyObj.transform.position);
        animator.SetLookAtWeight(1.0f, 0.0f, 1f, 0.0f, 0.8f);
    }

    #endregion

    #region ��e�֘A����

    void OnTriggerEnter(Collider col)
    {
        if (Deadflag || invincible) //���S���܂��͖��G���A������A�K�[�h���͏������Ȃ�
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

            //�G����̍U�����󂯂��Ƃ�
            if (col.CompareTag("AttackCollider"))
            {
                //�K�[�h�{�^����������Ă��āA�K�[�h���[�V�����̍Œ��ł�������
                if(_counterAttack)
                {
                    //�K�[�h�J�E���^�[�����s
                    animator.SetTrigger("CounterAttack");
                    audioManager.Play("Counter");

                    //�K�[�h�J�E���^�[�p�̍U���͂Ȃǂ����邽�߂��̃t���O���I����
                    //_counterFlag = true;
                    return;
                }
            }
        }
    }

    //��e���������pRPC
    void Damaged()
    {
        if (Gardflag)
        {
            return;
        }

        MoveLock = true;    //�d���̂��߈ړ����b�NON

        if (_damageReaction)
        {
            animator.SetTrigger("DamagedTrigger");  //�_���[�W�A�j���[�V����
            DamageSE();
        }
    }

    IEnumerator DamageReaction(float time)
    {
        _damageReaction = false;
        yield return new WaitForSeconds(time);
        _damageReaction = true;
    }

    //�q�b�g���d������
    IEnumerator _rigor(float pausetime)
    {
        yield return new WaitForSeconds(pausetime); //�|��Ă��鎞��
        MoveLock = false;   //�ړ����b�N����
    }

    public void Damage(float damage)
    {
        //������͖��G
        if (_avoidance || Deadflag)
        {
            return;
        }

        //��ʂ�ԓh��ɂ���
        img.color = new Color(0.5f, 0f, 0f, 0.5f);
        LocalVariables.currentHP -= damage;
        _bulkHPBar.value = LocalVariables.currentHP;
        _HPBar.DOValue(LocalVariables.currentHP, 0.5f);

        if (LocalVariables.currentHP <= 0)
        {
            Dead();
        }
        else
        {
            //�K�[�h��HP�͌��邯�ǁA��e���[�V�����͂��Ȃ�
            Damaged();
            StartCoroutine(DamageReaction(1));
            StartCoroutine(_rigor(.5f));    //��e�d������
        }
    }

    //���S���������pRPC
    void Dead()
    {
        AttackLock = true;  //�U�����b�NON
        MoveLock = true;    //�ړ����b�NON
        Deadflag = true;
        animator.SetTrigger("DeathTrigger");    //���S�A�j���[�V����ON
        audioManager.Play("GAMEOVER");
        GameSystem.Instance.SetGameState(GameSystem.GameState.GameOver);
    }

    public void ShakeUI()
    {
        GetComponent<PerlinNoiseController>().StartShake(0.3f, 100, 10);
    }

    #endregion

    #region �A�j���[�V�����C�x���g

    //�U���A�j���[�V�����J�n���ɌĂяo��
    public void StartAttack()
    {
        if(_counterAttack/*_counterFlag*/)
        {
            _counterCollider = true;
        }

        //���̓����蔻����I����
        Sword.GetComponent<Collider>().enabled = true;

        //���̋O�Ղ�\��
        _swordMiracle.SetActive(true);
    }

    //�U���A�j���[�V�����I�����ɌĂяo��
    public void EndAttack()
    {
        if(_counterCollider)
        {
            _counterCollider = false;
            //_counterFlag = false;
        }

        //���̓����蔻����I�t��
        Sword.GetComponent<Collider>().enabled = false;

        //���̋O�Ղ��\��
        _swordMiracle.SetActive(false);
    }

    public void EndCounterAttack()
    {
        //�J�E���^�[�A�^�b�N�t���O���I�t��
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
            GameSystem.Instance.SetGameState(GameSystem.GameState.GameClear);
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

    #region ���֘A

    //4��ނ���U�������������_���ɖ炵����
    void AttackSE()
    {
        int num = Random.Range(0, 4);

        switch(num)
        {
            case 0:
                audioManager.Play("Attack");
                break;
            case 1:
                audioManager.Play("Attack2");
                break;
            case 2:
                audioManager.Play("Attack3");
                break;
            case 3:
                audioManager.Play("Attack4");
                break;
        }
    }

    //3��ނ���_���[�W���������_���ɖ炵����
    void DamageSE()
    {
        int num = Random.Range(0, 3);

        switch (num)
        {
            case 0:
                audioManager.Play("GET_DAMAGE");
                break;
            case 1:
                audioManager.Play("GET_DAMAGE2");
                break;
            case 2:
                audioManager.Play("GET_DAMAGE3");
                break;
        }
    }

    #endregion

    #region �Q�b�^�[�@�Z�b�^�[

    //�G�l�~�[�I�u�W�F�N�g���Z�b�g
    public void SetEnemyObject(GameObject enemyObject)
    {
        enemyObj = enemyObject;
    }

    //�G�l�~�[�I�u�W�F�N�g���擾����֐�
    public GameObject GetEnemyObject()
    {
        return enemyObj;
    }

    #endregion
}

//�_���[�W�����C���^�[�t�F�C�X
interface IDamage
{
    //�_���[�W����
    public void Damage(float damage);

    public void ShakeUI();
}


//���N���X
public class Actor : MonoBehaviour
{
    protected CharacterControlScript Instance;
    protected GameSystem gameSystem;

    private void Awake()
    {
        Instance = FindObjectOfType<CharacterControlScript>();
        gameSystem = FindObjectOfType<GameSystem>();
    }
}