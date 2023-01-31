using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour//Photon.PunBehaviour/*, IPunObservable*/
{
    //�����UI��Prefab
    public GameObject PlayerUiPrefab;

    //���݂�HP
    public int HP = 100;

    //Local�̃v���C���[��ݒ�
    public static GameObject LocalPlayerInstance;

    //����UI�I�u�W�F�N�g
    GameObject _uiGo;

    #region �v���C���[�����ݒ�
    void Awake()
    {
        //if (photonView.isMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
    }
    #endregion

    #region ����UI�̐���
    void Start()
    {
        if (PlayerUiPrefab != null)
        {
            //Player�̓���UI�̐�����PlayerUIScript�ł�SetTarget�֐��ďo
            _uiGo = Instantiate(PlayerUiPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }
    #endregion

    void Update()
    {
        //if (!photonView.isMine) //���̃I�u�W�F�N�g��Local�łȂ���Ύ��s���Ȃ�
        //{
        //    return;
        //}
        //LocalVariables���Q�Ƃ��A���݂�HP���X�V
        HP = LocalVariables.currentHP;
    }

    #region OnPhotonSerializeView����
    //�v���C���[��HP,�`���b�g�𓯊�
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {
    //        stream.SendNext(this.HP);
    //        //stream.SendNext(this.ChatText);
    //    }
    //    else
    //    {
    //        this.HP = (int)stream.ReceiveNext();
    //        //this.ChatText = (string)stream.ReceiveNext();
    //    }
    //}
    #endregion
}
