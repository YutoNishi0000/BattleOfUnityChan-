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

        //もしもプレイヤー全員が準備完了ボタンを押したら
        if (_readyNum == PhotonNetwork.playerList.Length)
        {
            PhotonNetwork.LoadLevel("Fighting");
            //_ready = true;
            Debug.Log("シーンを移動します！！");
        }

        Debug.Log(PhotonNetwork.playerList.Length);

        //if (_ready)
        //{
        //    PhotonNetwork.LoadLevel("Fighting");
        //}
    }

    //準備ボタンを押したときの処理
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

    //一度だけしかボタンを押せないようにする
    public void OneClick()
    {
        //ボタンを押せないようにする
        GetComponent<Button>().interactable = false;
    }
}
