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

    //移動処理に必要なコンポーネントを設定
    public Animator animator;                 //モーションをコントロールするためAnimatorを取得
    public CharacterController controller;    //キャラクター移動を管理するためCharacterControllerを取得
    private PlayerAnimationEvents animEve;
    private PlayerSEController audioManager;

    public Slider _bulkHPBar;
    public Slider _HPBar;

    //移動速度等のパラメータ用変数(inspectorビューで設定)
    public float speed;         //キャラクターの移動速度
    public float jumpSpeed;     //キャラクターのジャンプ力
    public float rotateSpeed;   //キャラクターの方向転換速度
    public float gravity;       //キャラにかかる重力の大きさ
    public GameObject _avoidancePos;
    private float _avoidSpeed = 10f;
    Vector3 InitialPos;
    Vector3 avoidDirection;
    GameObject enemyObj;

    Vector3 targetDirection;        //移動する方向のベクトル
    Vector3 moveDirection = Vector3.zero;

    //戦闘用変数＆状態フラグ管理
    public GameObject Sword;                //自身が持っている剣
    bool MoveLock = false;                  //移動ロックフラグ
    bool AttackLock = false;                //連射防止用攻撃ロックフラグ
    bool invincible = false;                //無敵フラグ
    bool Deadflag = false;                  //死亡フラグ
    bool Gardflag = false;
    public bool _avoidance;/* = false*/                 //回避フラグ 
    public bool _counterAttack;
    public bool _counterCollider;
    //bool _counterFlag;
    bool _finishLock;
    bool _gardflag;
    bool _damageReaction;
    // 画面を赤にするためのイメージ
    public Image img;
    [SerializeField] private GameObject _swordMiracle;   //剣の軌跡エフェクト



    //　IKで角度を有効にするかどうか
    [SerializeField]
    private bool useIKRot = true;
    //　地面とするレイヤー
    [SerializeField]
    private LayerMask fieldLayer;
    //　右足のウエイト
    private float rightFootWeight = 0f;
    //　左足のウエイト
    private float leftFootWeight = 0f;
    //　右足の位置
    private Vector3 rightFootIKPosition;
    //　左足の位置
    private Vector3 leftFootIKPosition;
    //　右足の角度
    private Quaternion rightFootRot;
    //　左足の角度
    private Quaternion leftFootRot;
    //　右足と左足の距離
    private float distance;
    //　足を付く位置のオフセット値
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.06f, 0f);
    //　コライダの中心位置
    private Vector3 defaultCenter;
    //　レイを飛ばす距離
    [SerializeField]
    private float rayRange = 1f;

    //　体の重心を調整する時のスピード
    [SerializeField]
    private float bodyPositionSpeed = 50f;

    //　レイを飛ばす位置の調整値
    [SerializeField]
    private Vector3 rayPositionOffset = Vector3.up * 0.3f;

    //　体の重心を変更するかどうか
    [SerializeField]
    private bool isChangeBodyPosition = true;
    //　前回の体の重心位置
    private Vector3 preBodyPosition;
    //　足のレイが地面についているかどうか
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

        //移動ロックONまたは死亡フラグONであれば移動、攻撃をさせない
        if (!MoveLock && !Deadflag && !Gardflag)
        {
            moveControl();  //移動用関数
            RotationControl(); //旋回用関数
        }

        //攻撃ロックがかかっていなければ攻撃できる
        if (!AttackLock)
        {
            //攻撃処理
            AttackControl();
        }
    }

    #region 移動、回転処理

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

        //進行方向計算
        //キーボード入力を取得
        float v = Input.GetAxisRaw("Vertical");         //InputManagerの↑↓の入力       
        float h = Input.GetAxisRaw("Horizontal");       //InputManagerの←→の入力 

        //カメラの正面方向ベクトルからY成分を除き、正規化してキャラが走る方向を取得
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right; //カメラの右方向を取得

        //カメラの方向を考慮したキャラの進行方向を計算
        targetDirection = h * right + v * forward;

        rb.velocity = targetDirection * GetDashSpeed(speed);

        //走行アニメーション管理
        if (rb.velocity.magnitude > 0) //(移動入力があると)
        {
            animator.SetFloat("Speed", 1f); //キャラ走行のアニメーション開始
        }
        else    //(移動入力が無いと)
        {
            animator.SetFloat("Speed", 0); //キャラ走行のアニメーション終了
        }

    }

    /// <summary>
    /// プレイヤーの回転処理
    /// </summary>
    void RotationControl()  //キャラクターが移動方向を変えるときの処理
    {
        Vector3 rotateDirection = targetDirection;
        rotateDirection.y = 0;

        //それなりに移動方向が変化する場合のみ移動方向を変える
        if (rotateDirection.sqrMagnitude > 0.01)
        {
            //緩やかに移動方向を変える
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.Slerp(transform.forward, rotateDirection, step);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    float GetDashSpeed(float speed)
    {
        //もし左シフトを押した、またはスライディング回避中であれば
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //普段のスピードの1.5倍のスピードの値を返す
            return speed * 1.5f;
        }
        else
        {
            //特にダッシュ処理が行われていなければ
            return speed;
        }
    }

    #endregion

    #region ガード処理

    //
    void GardManager()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !Gardflag)
        {
            GardCounter();
            _counterAttack = true;
            audioManager.Play("Gard");

            //0.5秒後にカウンターフラグがオフに
            Invoke(nameof(EndCounterAttack), 3f);
            Debug.Log("カウンター攻撃のための判定開始");
        }
        else if (Input.GetKeyUp(KeyCode.Q) && Gardflag)
        {
            UnLockGard();
            CancelInvoke("UnLockGard");
        }
    }

    //ガード->タイミングがあればカウンター攻撃
    void GardCounter()
    {
        //フラグをオンに
        Gardflag = true;

        //各種アニメーション再生
        animator.SetFloat("ResetGard", 0);
        animator.SetFloat("Gard", 1);

        //ガード終わり
        Invoke(nameof(UnLockGard), 2);
    }

    void UnLockGard()
    {
        animator.SetFloat("ResetGard", 1);

        //フラグをオフに
        Gardflag = false;
    }

    #endregion

    #region 回避処理

    //回避(すぐに回避を繰り出すために値は０か1で調整)、入力管理は移動処理の中で行う、走っているときにしか繰り出せない、ボタンを押したら自動的に回避してくれるような関数を実装
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

    //回避が終わったときに回避に使った値などの初期化処理を行う
    public void UnLockAvoidance()
    {
        animator.ResetTrigger("Avoidance");
    }

    #endregion

    #region 攻撃処理

    void AttackControl()
    {
        if (Input.GetMouseButtonDown(0) && !animator.IsInTransition(0))	//　遷移途中でない
        {
            //攻撃ロック開始
            AttackLock = true;
            //移動ロック開始
            MoveLock = true;
            StartCoroutine(CAttack(1f));
            AttackSE();
        }
    }

    IEnumerator CAttack(float pausetime)
    {
        animator.SetTrigger("Attack");
        //攻撃硬直のためpausetimeだけ待つ
        yield return new WaitForSeconds(pausetime);
        //攻撃ロック解除
        AttackLock = false;
        //移動ロック解除
        MoveLock = false;
    }

    #endregion

    #region IK関連

    /// <summary>
    /// 顔のIKをモンスターの方向に向くように変更する
    /// </summary>
    public void OnAnimatorIK(int layerIndex)
    {
        if(_avoidance)
        {
            return;
        }

        //　アニメーションパラメータからIKのウエイトを取得
        rightFootWeight = animator.GetFloat("RightFootWeight");
        leftFootWeight = animator.GetFloat("LeftFootWeight");

        //　右足用のレイの視覚化
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot) + rayPositionOffset, Vector3.down * rayRange, Color.red);
        //　右足用のレイを飛ばす処理
        var ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + rayPositionOffset, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayRange, fieldLayer))
        {
            rightFootGrounded = true;
            rightFootIKPosition = hit.point;

            //　右足IKの設定
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

        //　左足用のレイを飛ばす処理
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + rayPositionOffset, Vector3.down);
        //　左足用のレイの視覚化
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + rayPositionOffset, Vector3.down * rayRange, Color.red);

        if (Physics.Raycast(ray, out hit, rayRange, fieldLayer))
        {
            leftFootGrounded = true;
            leftFootIKPosition = hit.point;

            //　左足IKの設定
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
        //　体の重心を動かす場合
        if (isChangeBodyPosition && rightFootGrounded && leftFootGrounded)
        {
            //　左右の足とキャラクターの足元の位置との距離を計算
            var rightFootDistance = rightFootIKPosition.y - transform.position.y;
            var leftFootDistance = leftFootIKPosition.y - transform.position.y;
            //　左右の足の位置がより下にある方を距離として使う
            var distance = rightFootDistance < leftFootDistance ? rightFootDistance : leftFootDistance;
            //　体の重心を下にある方の足に合わせて下げる
            var nowBodyPosition = animator.bodyPosition + Vector3.up * distance;
            //　徐々に変更するようにしているが、たぶんコメントにしてある処理のように一気に変えても問題ない
            animator.bodyPosition = Vector3.Lerp(preBodyPosition, nowBodyPosition, bodyPositionSpeed * Time.deltaTime);
            preBodyPosition = animator.bodyPosition;
            //animator.bodyPosition = nowBodyPosition;

        }



        //=====================================================================================================================
        // ここからは顔のIKを変更する処理に移る
        //=====================================================================================================================
        Vector3 target_pos = new Vector3(enemyObj.transform.position.x, 0, enemyObj.transform.position.z);
        Vector3 char_pos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = target_pos - char_pos;

        //水平方向でモンスター方向のベクトルと、自分が動いている方向のベクトルとの角度を取得
        float angle = Vector3.Angle(direction, moveDirection);

        //↑で求めた角度が150度以上であれば
        if (angle >= 150)
        {
            //ここからしたの処理は行わない
            return;
        }

        animator.SetLookAtPosition(enemyObj.transform.position);
        animator.SetLookAtWeight(1.0f, 0.0f, 1f, 0.0f, 0.8f);
    }

    #endregion

    #region 被弾関連処理

    void OnTriggerEnter(Collider col)
    {
        if (Deadflag || invincible) //死亡時または無敵時、回避時、ガード時は処理しない
        {
            return;
        }

        if(col.CompareTag("Ground"))
        {
            InitialPos = transform.position;
        }

        //当たった物がボールではないまたは自分が所持している剣ならなにもしない、またプレイヤーであっても何もしない
        if (col.CompareTag("Ball") || /*colAttacker.IsLocal || */col.CompareTag("Player") || col.CompareTag("Enemy") || col.CompareTag("Untagged"))
        {
            return;
        }
        else
        {
            ////ダメージを与える
            //LocalVariables.currentHP -= 10;

            //敵からの攻撃を受けたとき
            if (col.CompareTag("AttackCollider"))
            {
                //ガードボタンが押されていて、ガードモーションの最中であったら
                if(_counterAttack)
                {
                    //ガードカウンターを実行
                    animator.SetTrigger("CounterAttack");
                    audioManager.Play("Counter");

                    //ガードカウンター用の攻撃力などがあるためそのフラグをオンに
                    //_counterFlag = true;
                    return;
                }
            }
        }
    }

    //被弾処理同期用RPC
    void Damaged()
    {
        if (Gardflag)
        {
            return;
        }

        MoveLock = true;    //硬直のため移動ロックON

        if (_damageReaction)
        {
            animator.SetTrigger("DamagedTrigger");  //ダメージアニメーション
            DamageSE();
        }
    }

    IEnumerator DamageReaction(float time)
    {
        _damageReaction = false;
        yield return new WaitForSeconds(time);
        _damageReaction = true;
    }

    //ヒット時硬直処理
    IEnumerator _rigor(float pausetime)
    {
        yield return new WaitForSeconds(pausetime); //倒れている時間
        MoveLock = false;   //移動ロック解除
    }

    public void Damage(float damage)
    {
        //回避時は無敵
        if (_avoidance || Deadflag)
        {
            return;
        }

        //画面を赤塗りにする
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
            //ガード時HPは減るけど、被弾モーションはしない
            Damaged();
            StartCoroutine(DamageReaction(1));
            StartCoroutine(_rigor(.5f));    //被弾硬直処理
        }
    }

    //死亡処理同期用RPC
    void Dead()
    {
        AttackLock = true;  //攻撃ロックON
        MoveLock = true;    //移動ロックON
        Deadflag = true;
        animator.SetTrigger("DeathTrigger");    //死亡アニメーションON
        audioManager.Play("GAMEOVER");
        GameSystem.Instance.SetGameState(GameSystem.GameState.GameOver);
    }

    public void ShakeUI()
    {
        GetComponent<PerlinNoiseController>().StartShake(0.3f, 100, 10);
    }

    #endregion

    #region アニメーションイベント

    //攻撃アニメーション開始時に呼び出す
    public void StartAttack()
    {
        if(_counterAttack/*_counterFlag*/)
        {
            _counterCollider = true;
        }

        //剣の当たり判定をオンに
        Sword.GetComponent<Collider>().enabled = true;

        //剣の軌跡を表示
        _swordMiracle.SetActive(true);
    }

    //攻撃アニメーション終了時に呼び出す
    public void EndAttack()
    {
        if(_counterCollider)
        {
            _counterCollider = false;
            //_counterFlag = false;
        }

        //剣の当たり判定をオフに
        Sword.GetComponent<Collider>().enabled = false;

        //剣の軌跡を非表示
        _swordMiracle.SetActive(false);
    }

    public void EndCounterAttack()
    {
        //カウンターアタックフラグをオフに
        _counterAttack = false;
        Debug.Log("カウンター攻撃のための判定終了");
    }

    public void OnCounterCollider()
    {
        _counterCollider = true;
        //剣の当たり判定をオンに
        Sword.GetComponent<Collider>().enabled = true;
    }

    public void OffCounterCollider()
    {
        _counterCollider = false;
        //剣の当たり判定をオフに
        Sword.GetComponent<Collider>().enabled = false;
    }

    #endregion

    #region ゲームクリア、ゲームオーバー処理

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

    #region 音関連

    //4種類ある攻撃音声をランダムに鳴らしたい
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

    //3種類あるダメージ音をランダムに鳴らしたい
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

    #region ゲッター　セッター

    //エネミーオブジェクトをセット
    public void SetEnemyObject(GameObject enemyObject)
    {
        enemyObj = enemyObject;
    }

    //エネミーオブジェクトを取得する関数
    public GameObject GetEnemyObject()
    {
        return enemyObj;
    }

    #endregion
}

//ダメージ処理インターフェイス
interface IDamage
{
    //ダメージ処理
    public void Damage(float damage);

    public void ShakeUI();
}


//基底クラス
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