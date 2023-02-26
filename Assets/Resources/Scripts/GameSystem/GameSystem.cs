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

    public GameObject[] _monstersUI;

    public static int DeathMonsterNum;    //�|���������X�^�[�̐�

    public bool _instantiateLock;    //�����X�^�[�̐��������b�N���邽�߂̃t���O

    public CameraShakeController _shake;

    public enum GameState
    {
        Battle, 
        GameClear,
        GameOver
    }

    public GameState _state;

    public Text _gameClear;
    public Text _gameOver;
    public GameObject _button;

    // Start is called before the first frame update
    void Start()
    {
        _shake = GetComponent<CameraShakeController>();
        _state = new GameState();

        DeathMonsterNum = 0;
        GenerateMonster(DeathMonsterNum);

        _gameClear.enabled = false;
        _gameOver.enabled = false;
        _button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(_state)
        {
            //�Q�[���N���A��������
            case GameState.GameClear:
                Cursor.lockState = CursorLockMode.None;
                _gameClear.enabled = true;
                _button.SetActive(true);
                break;
            //�Q�[���I�[�o�[��������
            case GameState.GameOver:
                Cursor.lockState = CursorLockMode.None;
                _gameOver.enabled = true;
                _button.SetActive(true);
                break;
        }
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