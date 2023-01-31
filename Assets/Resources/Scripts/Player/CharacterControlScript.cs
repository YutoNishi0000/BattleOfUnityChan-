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
    bool _counterFlag;
    bool _finishLock;

    // Start is called before the first frame update
    void Start()
    {
        _counterAttack = false;
        _counterCollider = false;
        _counterFlag = false;
        _finishLock = false;

        //if (myPV.isMine)    //自キャラであれば実行
        {
            //MainCameraのtargetにこのゲームオブジェクトを設定
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

        //移動ロックONまたは死亡フラグONであれば移動、攻撃をさせない
        if (!MoveLock && !Deadflag && !Gardflag)
        {
            moveControl();  //移動用関数
            RotationControl(); //旋回用関数
            if (_avoidance)
            {
                //AvoidanceManager();//回避用関数
                Avoidance();
                DOTween.To(() => controller.height, (val) => { controller.height = val; }, 0, 0.3f);

                if (controller.velocity.magnitude <= 0)
                {
                    UnLockAvoidance();
                }
            }
            //最終的な移動処理
            //(これが無いとCharacterControllerに情報が送られないため、動けない)
            controller.Move(moveDirection * Time.deltaTime);
        }

        //攻撃ロックがかかっていなければ攻撃できる
        if (!AttackLock)
        {
            //攻撃処理
            AttackControl();
        }
    }

    void moveControl()
    {
        //★進行方向計算
        //キーボード入力を取得
        float v = Input.GetAxisRaw("Vertical");         //InputManagerの↑↓の入力       
        float h = Input.GetAxisRaw("Horizontal");       //InputManagerの←→の入力 

        //カメラの正面方向ベクトルからY成分を除き、正規化してキャラが走る方向を取得
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right; //カメラの右方向を取得

        //カメラの方向を考慮したキャラの進行方向を計算
        targetDirection = h * right + v * forward;

        //地上にいる場合の処理
        if (controller.isGrounded)
        {
            //移動のベクトルを計算
            moveDirection = targetDirection * speed;

            //Jumpボタンでジャンプ処理
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else        //空中操作の処理（重力加速度等）
        {
            float tempy = moveDirection.y;
            //(↓の２文の処理があると空中でも入力方向に動けるようになる)
            //moveDirection = Vector3.Scale(targetDirection, new Vector3(1, 0, 1)).normalized;
            //moveDirection *= speed;
            moveDirection.y = tempy - gravity * Time.deltaTime;
        }

        //走行アニメーション管理
        if (v > .1 || v < -.1 || h > .1 || h < -.1) //(移動入力があると)
        {
            animator.SetFloat("Speed", 1f); //キャラ走行のアニメーション開始

            //回避フラグがオフの状態で走行中にEボタンを押したら
            if(!_avoidance && Input.GetKeyDown(KeyCode.E))
            {
                _avoidance = true;
                avoidDirection = transform.forward;
            }
        }
        else    //(移動入力が無いと)
        {
            animator.SetFloat("Speed", 0); //キャラ走行のアニメーション終了
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
            Debug.Log("カウンター攻撃のための判定開始");
        }
        else if (Input.GetKeyUp(KeyCode.Q) && Gardflag)
        {
            UnLockGard();
            CancelInvoke("UnLockGard");
        }
    }

    //ガード->タイミングがあればカウンター攻撃
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

    //回避を行う関数を実際に実行する関数
    void AvoidanceManager()
    {
        //同期を行う
        //myPV.RPC("Avoidance", PhotonTargets.AllViaServer);
    }

    //回避(すぐに回避を繰り出すために値は０か1で調整)、入力管理は移動処理の中で行う、走っているときにしか繰り出せない、ボタンを押したら自動的に回避してくれるような関数を実装
    //回避開始時のモーションと回避解除時のモーションを同期させたいためここで一通りの回避処理を記述している。
    //[PunRPC]
    void Avoidance()
    {
        //if (_avoidance)
        {
            animator.SetFloat("ResetAvoidance", 0);
            animator.SetFloat("Avoidance", 1);
        }

        //一秒後に回避解除したい
        Invoke(nameof(UnLockAvoidance), 0.6f);
    }

    void UnLockAvoidance()
    {
        //回避解除
        animator.SetFloat("ResetAvoidance", 1);
        _avoidance = false;
        //引数はラムダ式を使って変数を設定
        DOTween.To(() => controller.height, (val) => { controller.height = val; }, 2, 1f);
    }

    void RotationControl()  //キャラクターが移動方向を変えるときの処理
    {
        Vector3 rotateDirection = moveDirection;
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

    //ボール攻撃
    void AttackControl()
    {
        if (Input.GetMouseButtonDown(0) && !animator.IsInTransition(0))	//　遷移途中でない
        {
            //攻撃ロック開始
            AttackLock = true;
            //移動ロック開始
            MoveLock = true;
            StartCoroutine(_ballattack(1f));
        }
    }

    IEnumerator _ballattack(float pausetime)
    {
        //RPCでボール生成
        //myPV.RPC("BallInst", PhotonTargets.AllViaServer, transform.position + transform.up, transform.rotation);
        animator.SetTrigger("Attack");
        //攻撃硬直のためpausetimeだけ待つ
        yield return new WaitForSeconds(pausetime);
        //攻撃ロック解除
        AttackLock = false;
        //移動ロック解除
        MoveLock = false;
    }

    #region 被弾関連処理

    void OnTriggerEnter(Collider col)
    {
        if (Deadflag || invincible || _avoidance) //死亡時または無敵時、回避時は処理しない
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

    //被弾処理同期用RPC
    //[PunRPC]
    void Damaged()
    {
        MoveLock = true;    //硬直のため移動ロックON
        animator.SetTrigger("DamagedTrigger");  //ダメージアニメーション
    }

    //ヒット時硬直処理
    IEnumerator _rigor(float pausetime)
    {
        yield return new WaitForSeconds(pausetime); //倒れている時間
        MoveLock = false;   //移動ロック解除
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
            StartCoroutine(_rigor(.5f));    //被弾硬直処理
        }
    }

    //死亡処理同期用RPC
    void Dead()
    {
        if(Deadflag)
        {
            return;
        }

        Deadflag = true;    //死亡フラグON
        AttackLock = true;  //攻撃ロックON
        MoveLock = true;    //移動ロックON
        animator.SetTrigger("DeathTrigger");    //死亡アニメーションON
    }

    //復活コルーチン
    IEnumerator _revive(float pausetime)
    {
        yield return new WaitForSeconds(pausetime); //倒れている時間
        //復活
        Deadflag = false;   //死亡解除
        AttackLock = false; //攻撃ロック解除
        MoveLock = false;   //移動ロック解除
        invincible = true;  //死亡後無敵開始
        LocalVariables.currentHP = 100; //HP回復
        yield return new WaitForSeconds(5f);    //死亡後無敵時間
        invincible = false; //無敵解除
    }
    #endregion

    #region アニメーションイベント

    //攻撃アニメーション開始時に呼び出す
    public void StartAttack()
    {
        if(_counterFlag)
        {
            _counterCollider = true;
        }

        //剣の当たり判定をオンに
        Sword.GetComponent<Collider>().enabled = true;
    }

    //攻撃アニメーション終了時に呼び出す
    public void EndAttack()
    {
        if(_counterCollider)
        {
            _counterCollider = false;
            _counterFlag = false;
        }

        //剣の当たり判定をオフに
        Sword.GetComponent<Collider>().enabled = false;
    }

    public void EndCounterAttack()
    {
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

//ダメージ処理インターフェイス
interface IDamage
{
    //ダメージ処理
    public void Damage(int damage);
}


//基底クラス
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