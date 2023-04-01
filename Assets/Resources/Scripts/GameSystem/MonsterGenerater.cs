using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//このシーン内だけのシングルトンインスタンスクラス
public class MonsterGenerater : MonoBehaviour
{
    public static MonsterGenerater generater;

    public GameObject[] _monsters;

    private void Awake()
    {
        if (generater == null)
        {
            generater = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameSystem.Instance.Initialization();
        GenerateMonster(GameSystem.DeathMonsterNum);
    }

    public void GenerateMonster(int number)
    {
        Instantiate(_monsters[number], transform.position, Quaternion.identity);
    }
}
