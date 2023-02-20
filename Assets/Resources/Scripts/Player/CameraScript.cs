using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private const float YAngle_MIN = -89.0f;   //�J������Y�����̍ŏ��p�x
    private const float YAngle_MAX = 89.0f;     //�J������Y�����̍ő�p�x

    public Transform target;    //�ǐՂ���I�u�W�F�N�g��transform
    public Vector3 offset;      //�ǐՑΏۂ̒��S�ʒu�����p�I�t�Z�b�g
    private Vector3 lookAt;     //target��offset�ɂ�钍��������W

    private float distance = 3.0f;    //�L�����N�^�[�ƃJ�����Ԃ̊p�x
    private float DEFAULT_DISTANCE = 3.0f;
    private float distance_min = 0.0f;  //�L�����N�^�[�Ƃ̍ŏ�����
    private float distance_max = 20.0f; //�L�����N�^�[�Ƃ̍ő勗��
    private float currentX = 0.0f;  //�J������X�����ɉ�]������p�x
    private float currentY = 0.0f;  //�J������Y�����ɉ�]������p�x

    //�J������]�p�W��(�l���傫���قǉ�]���x���オ��)
    private float moveX = 4.0f;     //�}�E�X�h���b�O�ɂ��J����X������]�W��
    private float moveY = 2.0f;     //�}�E�X�h���b�O�ɂ��J����Y������]�W��
    private float moveX_QE = 2.0f;  //QE�L�[�ɂ��J����X������]�W��

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
        //�}�E�X�E�N���b�N�������Ă���Ƃ������}�E�X�̈ړ��ʂɉ����ăJ��������]
        currentX += Input.GetAxis("Mouse X") * moveX;
        currentY += Input.GetAxis("Mouse Y") * moveY;
        currentY = Mathf.Clamp(currentY, YAngle_MIN, YAngle_MAX);
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distance_min, distance_max);
        if (target != null)  //target���w�肳���܂ł̃G���[���
        {
            var targetVec = new Vector3(target.position.x, 0, target.position.z) + offset;
            lookAt = target.position + offset;  //�������W��target�ʒu+offset�̍��W

            //�J�������񏈗�
            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);

            //transform.DOMove(lookAt + rotation * dir, 0.5f);
            transform.LookAt(lookAt);   //�J������LookAt�̕����Ɍ���������
            //transform.position = lookAt + rotation * dir;   //�J�����̈ʒu��ύX
            //�J�����̕ǂ��蔲����h��
            RaycastHit hit;
            Vector3 cameraPos;
            if (Physics.Raycast(target.transform.position, transform.position - target.transform.position, out hit, distance/*, LayerMask.NameToLayer("Player")*/))
            {
                Debug.Log("�ǂɓ������Ă��");
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