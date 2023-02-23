using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class GameSystem : MonoBehaviour
{
    #region �V���O���g����

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

    public static int DeathMonsterNum;    //�|���������X�^�[�̐�

    public bool _instantiateLock;    //�����X�^�[�̐��������b�N���邽�߂̃t���O

    public CameraShakeController _shake;

    // Start is called before the first frame update
    void Start()
    {
        ////�S�Ẵ����X�^�[UI��������
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
        //    //�S�Ẵ����X�^�[UI�𒲂׈����Ŏw�肳�ꂽ�ԍ��Ɠ����ł����
        //    if (i == number)
        //    {
        //        //�w�肳�ꂽ�����X�^�[UI��\������
        //        _monstersUI[number].SetActive(true);
        //    }
        //    //�����łȂ����
        //    else
        //    {
        //        //�����X�^�[UI���\���ɂ���
        //        _monstersUI[i].SetActive(false);
        //    }
        //}

        Instantiate(_monsters[number], transform.position, Quaternion.identity);
    }
}