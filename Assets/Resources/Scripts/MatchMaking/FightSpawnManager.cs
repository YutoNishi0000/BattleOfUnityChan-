using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSpawnManager : Photon.PunBehaviour
{
    public GameObject[] spawnPos;

    public GameObject playerPref;

    private void Start()
    {
        if (!PhotonNetwork.connected)   //Phootn�ɐڑ�����Ă��Ȃ����
        {
            SceneManager.LoadScene("Launcher"); //���O�C����ʂɖ߂�
            return;
        }

        //Photon�ɐڑ����Ă���Ύ��v���C���[�𐶐�
        GameObject Player = PhotonNetwork.Instantiate(this.playerPref.name, spawnPos[PhotonNetwork.player.ID - 1].transform.position, Quaternion.identity, 0);
    }
}
