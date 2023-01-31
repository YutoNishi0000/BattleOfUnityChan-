using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherScript : Photon.PunBehaviour
{
    #region Public変数定義

    //Public変数の定義はココで

    #endregion

    #region Private変数
    //Private変数の定義はココで
    string _gameVersion = "Chapter12";   //ゲームのバージョン。仕様が異なるバージョンとなったときはバージョンを変更しないとエラーが発生する。
    #endregion

    #region Public Methods
    //ログインボタンを押したときに実行される
    public void Connect()
    {
        //if (!PhotonNetwork.connected)
        //{                         //Photonに接続できていなければ
        //    PhotonNetwork.ConnectUsingSettings(_gameVersion);   //Photonに接続する
        //    Debug.Log("Photonに接続しました。");
        //    SceneManager.LoadScene("Lobby");    //Lobbyシーンに遷移
        //}

        SceneManager.LoadScene("Fighting");
    }
    #endregion
}
