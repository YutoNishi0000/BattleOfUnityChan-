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
        if (!PhotonNetwork.connected)   //Phootnに接続されていなければ
        {
            SceneManager.LoadScene("Launcher"); //ログイン画面に戻る
            return;
        }

        //Photonに接続していれば自プレイヤーを生成
        GameObject Player = PhotonNetwork.Instantiate(this.playerPref.name, spawnPos[PhotonNetwork.player.ID - 1].transform.position, Quaternion.identity, 0);
    }
}
