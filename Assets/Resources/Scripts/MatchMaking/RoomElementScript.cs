using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomElementScript : MonoBehaviour
{
    //Room���UI�\���p
    public Text RoomName;   //������
    public Text PlayerNumber;   //�l��
    public Text RoomCreator;    //�����쐬�Җ�

    //�����{�^��roomname�i�[�p
    private string roomname;

    //GetRoomList����Room����RoomElement�ɃZ�b�g���Ă������߂̊֐�
    public void SetRoomInfo(string _RoomName, int _PlayerNumber, int _MaxPlayer, string _RoomCreator)
    {
        //�����{�^���proomname�擾
        roomname = _RoomName;
        RoomName.text = "�������F" + _RoomName;
        PlayerNumber.text = "�l�@���F" + _PlayerNumber + "/" + _MaxPlayer;
        RoomCreator.text = "�쐬�ҁF" + _RoomCreator;
    }

    //�����{�^������
    public void OnJoinRoomButton()
    {
        //roomname�̕����ɓ���
        PhotonNetwork.JoinRoom(roomname);
    }
}
