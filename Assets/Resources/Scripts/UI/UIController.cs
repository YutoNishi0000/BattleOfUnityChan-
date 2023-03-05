using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text _gameClear;
    public Text _gameOver;
    public GameObject _button;

    // Start is called before the first frame update
    void Start()
    {
        _gameClear.enabled = false;
        _gameOver.enabled = false;
        _button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameSystem.Instance.GetGameState())
        {
            //ゲームクリアだったら
            case GameSystem.GameState.GameClear:
                Cursor.lockState = CursorLockMode.None;
                _gameClear.enabled = true;
                _button.SetActive(true);
                break;
            //ゲームオーバーだったら
            case GameSystem.GameState.GameOver:
                Cursor.lockState = CursorLockMode.None;
                _gameOver.enabled = true;
                _button.SetActive(true);
                break;
        }
    }
}
