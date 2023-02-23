using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class GameSystem : MonoBehaviour
{
    #region シングルトン化

    public static GameSystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public GameObject[] _monsters;

    public GameObject[] _monstersUI;

    public static int DeathMonsterNum;    //倒したモンスターの数

    public bool _instantiateLock;    //モンスターの生成をロックするためのフラグ

    public CameraShakeController _shake;

    // Start is called before the first frame update
    void Start()
    {
        ////全てのモンスターUIを初期化
        //for(int i = 0; i < _monstersUI.Length; i++)
        //{
        //    _monstersUI[i].SetActive(false);
        //}
        _shake = GetComponent<CameraShakeController>();

        DeathMonsterNum = 0;
        GenerateMonster(DeathMonsterNum);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMonster(int number)
    {
        //for(int i = 0; i < _monstersUI.Length; i++)
        //{
        //    //全てのモンスターUIを調べ引数で指定された番号と同じであれば
        //    if (i == number)
        //    {
        //        //指定されたモンスターUIを表示する
        //        _monstersUI[number].SetActive(true);
        //    }
        //    //そうでなければ
        //    else
        //    {
        //        //モンスターUIを非表示にする
        //        _monstersUI[i].SetActive(false);
        //    }
        //}

        Instantiate(_monsters[number], transform.position, Quaternion.identity);
    }
}