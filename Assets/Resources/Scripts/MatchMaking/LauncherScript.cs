using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherScript : Photon.PunBehaviour
{
    //ログインボタンを押したときに実行される
    public void Connect()
    {
        SceneManager.LoadScene("Fighting");
    }

    public void Return()
    {
        SceneManager.LoadScene("Launcher");
    }
}
