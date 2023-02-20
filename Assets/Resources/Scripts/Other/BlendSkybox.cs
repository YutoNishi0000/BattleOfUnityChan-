using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendSkybox : MonoBehaviour
{
    public static bool _night;
    [SerializeField] Material _skyMat = default;
    [SerializeField, Range(0f, 1f)] float _rate = 0.002f;
    Coroutine _coroutine = default;
    Material _runtimeMaterial = default;
    private float InitialReflectionIntensity;
    private Color InitialAmbientSkyColor;
    [SerializeField] private Light playerLight;

    private void Start()
    {
        _night = false;
    }

    public void FadeSkybox()
    {
        //if (_coroutine == null)
        {
            _runtimeMaterial = Instantiate(_skyMat);
            RenderSettings.skybox = _runtimeMaterial;
            /*_coroutine = */StartCoroutine(FadeSkyboxRoutine(_rate));
        }
    }

    public void InitializeSkybox()
    {
        //if (_coroutine == null)
        {
            _runtimeMaterial = Instantiate(_skyMat);
            RenderSettings.skybox = _runtimeMaterial;
            /*_coroutine = */StartCoroutine(InitializeSkyboxRoutine(_rate));
        }
    }

    IEnumerator FadeSkyboxRoutine(float rate)
    {
        float blend = 0f;

        while (blend < 1)
        {
            _runtimeMaterial.SetFloat("_Blend", blend);
            blend += rate;

            yield return null;

            if (playerLight.intensity <= 0.05f)
            {
                continue;
            }

            playerLight.intensity -= rate;
        }

        _runtimeMaterial.SetFloat("_Blend", 1);
        Debug.Log("Fade Finished.");
        //_coroutine = null;
    }

    IEnumerator InitializeSkyboxRoutine(float rate)
    {
        float blend = 1f;

        while (blend > 0)
        {
            _runtimeMaterial.SetFloat("_Blend", blend);
            blend -= rate;

            yield return null;

            if (playerLight.intensity >= 1)
            {
                playerLight.intensity = 1;
                continue;
            }

            playerLight.intensity += rate;
        }

        _runtimeMaterial.SetFloat("_Blend", 0);
        Debug.Log("Fade Finished.");
        //_coroutine = null;
    }
}
