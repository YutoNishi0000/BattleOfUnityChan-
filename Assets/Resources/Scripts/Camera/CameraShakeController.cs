using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _freeLookCamera;
    [SerializeField] private NoiseSettings.NoiseParams noiseParams;
    private bool _startNoise;
    float _erapsedTime;
    float duration;
    float _strength;

    private void Start()
    {
        _startNoise = false;
        noiseParams.Amplitude = 1;
        noiseParams.Frequency = 1;
    }

    private void Update()
    {
        //if(_startNoise)
        //{
        //    _erapsedTime += Time.deltaTime;

        //    if (_erapsedTime < duration)
        //    {
        //        noiseParams.Amplitude = 1;
        //        noiseParams.Frequency = _strength;
        //    }
        //    else
        //    {
        //        _erapsedTime = 0;
        //        noiseParams.Amplitude = 0;
        //        _startNoise = false;
        //    }
        //}
    }

    public void Shake(float duaration, float strength, float time)
    {
        duration = duaration;
        _strength = strength;
        _startNoise = true;
    }
}