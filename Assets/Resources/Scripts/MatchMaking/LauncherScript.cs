using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherScript : Photon.PunBehaviour
{
    #region Public�ϐ���`

    //Public�ϐ��̒�`�̓R�R��

    #endregion

    #region Private�ϐ�
    //Private�ϐ��̒�`�̓R�R��
    string _gameVersion = "Chapter12";   //�Q�[���̃o�[�W�����B�d�l���قȂ�o�[�W�����ƂȂ����Ƃ��̓o�[�W������ύX���Ȃ��ƃG���[����������B
    #endregion

    #region Public Methods
    //���O�C���{�^�����������Ƃ��Ɏ��s�����
    public void Connect()
    {
        //if (!PhotonNetwork.connected)
        //{                         //Photon�ɐڑ��ł��Ă��Ȃ����
        //    PhotonNetwork.ConnectUsingSettings(_gameVersion);   //Photon�ɐڑ�����
        //    Debug.Log("Photon�ɐڑ����܂����B");
        //    SceneManager.LoadScene("Lobby");    //Lobby�V�[���ɑJ��
        //}

        SceneManager.LoadScene("Fighting");
    }
    #endregion
}
