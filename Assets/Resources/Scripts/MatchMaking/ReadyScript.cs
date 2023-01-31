using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReadyScript : MonoBehaviour
{
    private PhotonView _photonView;

    [SerializeField]
    private int _readyNum;
    private bool _ready;

    void Awake()
    {
        _readyNum = 0;
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        Debug.Log(_readyNum);

        //�������v���C���[�S�������������{�^������������
        if (_readyNum == PhotonNetwork.playerList.Length)
        {
            PhotonNetwork.LoadLevel("Fighting");
            //_ready = true;
            Debug.Log("�V�[�����ړ����܂��I�I");
        }

        Debug.Log(PhotonNetwork.playerList.Length);

        //if (_ready)
        //{
        //    PhotonNetwork.LoadLevel("Fighting");
        //}
    }

    //�����{�^�����������Ƃ��̏���
    public void ReadyToStart()
    {
        _photonView.RPC("IncrementReadyNum", PhotonTargets.AllViaServer);
        Debug.Log(_readyNum);
    }

    [PunRPC]
    public void IncrementReadyNum()
    {
        _readyNum++;
    }

    //��x���������{�^���������Ȃ��悤�ɂ���
    public void OneClick()
    {
        //�{�^���������Ȃ��悤�ɂ���
        GetComponent<Button>().interactable = false;
    }
}
