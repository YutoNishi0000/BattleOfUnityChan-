using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : Photon.PunBehaviour
{
    //�N�������O�C������x�ɐ�������v���C���[Prefab
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.sceneCount == 4)   //Phootn�ɐڑ�����Ă��Ȃ����
        {
            SceneManager.LoadScene("Launcher"); //���O�C����ʂɖ߂�
            return;
        }

        ////Photon�ɐڑ����Ă���Ύ��v���C���[�𐶐�
        //GameObject Player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
    }
}