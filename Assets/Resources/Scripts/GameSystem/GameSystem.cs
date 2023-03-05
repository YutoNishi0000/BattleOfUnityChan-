using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

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

    public static int DeathMonsterNum;    //�|���������X�^�[�̐�

    public bool _instantiateLock;    //�����X�^�[�̐��������b�N���邽�߂̃t���O

    public enum GameState
    {
        Battle, 
        GameClear,
        GameOver
    }

    public GameState _state;

    // Start is called before the first frame update
    void Start()
    {
        _state = new GameState();

        DeathMonsterNum = 0;
        GenerateMonster(DeathMonsterNum);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void JudgeGameClear()
    {
        if(DeathMonsterNum >= 4)
        {

        }
    }

    public void GenerateMonster(int number)
    {
        Instantiate(_monsters[number], transform.position, Quaternion.identity);
    }

    public void SetGameState(GameState state)
    {
        _state = state;
    }

    public GameState GetGameState()
    {
        return _state;
    }
}