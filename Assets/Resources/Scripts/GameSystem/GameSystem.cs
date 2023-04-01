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
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Initialization()
    {
        DeathMonsterNum = 0;
        _state = new GameState();
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