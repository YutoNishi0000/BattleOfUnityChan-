using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : Actor
{
    public AudioClip[] _audioObjects;

    private AudioSource _audioSource;

    private int _passCount;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(GameSystem.Instance.GetGameState())
        {
            case GameSystem.GameState.GameClear:
                _audioSource.Stop();
                break;
            case GameSystem.GameState.GameOver:
                _audioSource.Stop();
                break;
        }

        switch (GameSystem.DeathMonsterNum)
        {
            case 0:
                if(_passCount >= 1)
                {
                    return;
                }

                _audioSource.Stop();
                _audioSource.clip = _audioObjects[0];
                _audioSource.Play();
                _audioSource.loop = true;
                _passCount++;
                break;
            case 1:
                if(_passCount >= 2)
                {
                    return;
                }

                _audioSource.Stop();
                _audioSource.clip = _audioObjects[1];
                _audioSource.Play();
                _audioSource.loop = true;
                _passCount++;
                break;
            case 2:
                if (_passCount >= 3)
                {
                    return;
                }

                _audioSource.Stop();
                _audioSource.clip = _audioObjects[2];
                _audioSource.Play();
                _audioSource.loop = true;
                _passCount++;
                break;
            case 3:
                if(_passCount >= 4)
                {
                    return;
                }

                _audioSource.Stop();
                _audioSource.clip = _audioObjects[3];
                _audioSource.Play();
                _audioSource.loop = true;
                _passCount++;
                break;
            case 4:
                _audioSource.Stop();
                break;
        }
    }
}