using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : Photon.PunBehaviour
{
    //誰かがログインする度に生成するプレイヤーPrefab
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.sceneCount == 4)   //Phootnに接続されていなければ
        {
            SceneManager.LoadScene("Launcher"); //ログイン画面に戻る
            return;
        }

        ////Photonに接続していれば自プレイヤーを生成
        //GameObject Player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
    }
}