using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private const float YAngle_MIN = -89.0f;   //カメラのY方向の最小角度
    private const float YAngle_MAX = 89.0f;     //カメラのY方向の最大角度

    public Transform target;    //追跡するオブジェクトのtransform
    public Vector3 offset;      //追跡対象の中心位置調整用オフセット
    private Vector3 lookAt;     //targetとoffsetによる注視する座標

    private float distance = 3.0f;    //キャラクターとカメラ間の角度
    private float DEFAULT_DISTANCE = 3.0f;
    private float distance_min = 0.0f;  //キャラクターとの最小距離
    private float distance_max = 20.0f; //キャラクターとの最大距離
    private float currentX = 0.0f;  //カメラをX方向に回転させる角度
    private float currentY = 0.0f;  //カメラをY方向に回転させる角度

    //カメラ回転用係数(値が大きいほど回転速度が上がる)
    private float moveX = 4.0f;     //マウスドラッグによるカメラX方向回転係数
    private float moveY = 2.0f;     //マウスドラッグによるカメラY方向回転係数
    private float moveX_QE = 2.0f;  //QEキーによるカメラX方向回転係数

    [SerializeField] private float CAMERA_SENSITIVILITY = 0.5F;   

    // Start is called before the first frame update
    void Start()
    {
        currentX += 180;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //マウス右クリックを押しているときだけマウスの移動量に応じてカメラが回転
        currentX += Input.GetAxis("Mouse X") * moveX;
        currentY += Input.GetAxis("Mouse Y") * moveY;
        currentY = Mathf.Clamp(currentY, YAngle_MIN, YAngle_MAX);
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distance_min, distance_max);
        if (target != null)  //targetが指定されるまでのエラー回避
        {
            var targetVec = new Vector3(target.position.x, 0, target.position.z) + offset;
            lookAt = target.position + offset;  //注視座標はtarget位置+offsetの座標

            //カメラ旋回処理
            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);

            //transform.DOMove(lookAt + rotation * dir, 0.5f);
            transform.LookAt(lookAt);   //カメラをLookAtの方向に向けさせる
            //transform.position = lookAt + rotation * dir;   //カメラの位置を変更
            //カメラの壁すり抜けを防ぐ
            RaycastHit hit;
            Vector3 cameraPos;
            if (Physics.Raycast(target.transform.position, transform.position - target.transform.position, out hit, distance/*, LayerMask.NameToLayer("Player")*/))
            {
                Debug.Log("壁に当たってるよ");
                //transform.DOMove(hit.transform.position, 0.5f);
                //
                //var dis = Quaternion.Euler(hit.point - transform.position) * Vector3.back;
                //cameraPos = hit.point + rotation * dis;
                DOTween.To(() => distance, (val) => { distance = val; }, (hit.point - transform.position).magnitude - 0.5f, 0.1f);
            }
            else
            {
                //transform.DOMove(lookAt + rotation * dir, 0.5f);
                //cameraPos = lookAt + rotation * dir;
                DOTween.To(() => distance, (val) => { distance = val; }, DEFAULT_DISTANCE, 1f);
            }

            cameraPos = lookAt + rotation * dir;

            transform.DOMove(cameraPos, CAMERA_SENSITIVILITY);
        }
    }
}