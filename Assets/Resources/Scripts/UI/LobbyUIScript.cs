using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIScript : MonoBehaviour
{

    //�����쐬�E�C���h�E�\���p�{�^��
    public Button OpenRoomPanelButton;

    //�����쐬�E�C���h�E
    public GameObject CreateRoomPanel;  //�����쐬�E�C���h�E
    public Text RoomNameText;           //�쐬���镔����
    public Slider PlayerNumberSlider;   //�ő�����\�l���pSlider
    public Text PlayerNumberText;       //�ő�����\�l���\���pText
    public Button CreateRoomButton;     //�����쐬�{�^��

    // Update is called once per frame
    void Update()
    {
        //�����l��Slider�̒l��Text�ɑ��
        PlayerNumberText.text = PlayerNumberSlider.value.ToString();
    }

    //�����쐬�E�C���h�E�\���p�{�^�����������Ƃ��̏���
    public void OnClick_OpenRoomPanelButton()
    {
        //�����쐬�E�C���h�E���\�����Ă����
        if (CreateRoomPanel.activeSelf)
        {
            //�����쐬�E�C���h�E���\����
            CreateRoomPanel.SetActive(false);
        }
        else //�����łȂ����
        {
            //�����쐬�E�C���h�E��\��
            CreateRoomPanel.SetActive(true);
        }
    }

    //�����쐬�{�^�����������Ƃ��̏���
    public void OnClick_CreateRoomButton()
    {
        //�쐬���镔���̐ݒ�
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;   //���r�[�Ō����镔���ɂ���
        roomOptions.IsOpen = true;      //���̃v���C���[�̓�����������
        roomOptions.MaxPlayers = (byte)PlayerNumberSlider.value;    //�����\�l����ݒ�
        //���[���J�X�^���v���p�e�B�ŕ����쐬�҂�\�������邽�߁A�쐬�҂̖��O���i�[
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "RoomCreator",PhotonNetwork.playerName }
        };
        //���r�[�ɃJ�X�^���v���p�e�B�̏���\��������
        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            "RoomCreator",
        };

        //�����쐬
        PhotonNetwork.CreateRoom(RoomNameText.text, roomOptions, null);
    }
}
