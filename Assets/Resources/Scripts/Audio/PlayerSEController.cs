using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSEController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        //コンポーネント取得
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //効果音を再生する
    public void Play(string seName)
    {
        switch (seName)
        {
            case "Run":
                audioSource.PlayOneShot(audioClips[0]);
                break;
            case "Attack":
                audioSource.PlayOneShot(audioClips[1]);
                break;
            case "Attack2":
                audioSource.PlayOneShot(audioClips[2]);
                break;
            case "Attack3":
                audioSource.PlayOneShot(audioClips[3]);
                break;
            case "Attack4":
                audioSource.PlayOneShot(audioClips[4]);
                break;
            case "GET_DAMAGE":
                audioSource.PlayOneShot(audioClips[5]);
                break;
            case "GET_DAMAGE2":
                audioSource.PlayOneShot(audioClips[6]);
                break;
            case "GET_DAMAGE3":
                audioSource.PlayOneShot(audioClips[7]);
                break;
            case "Avoidance":
                audioSource.PlayOneShot(audioClips[8]);
                break;
            case "Gard":
                audioSource.PlayOneShot(audioClips[9]);
                break;
            case "Counter":
                audioSource.PlayOneShot(audioClips[10]);
                break;
            case "GAMEOVER":
                audioSource.PlayOneShot(audioClips[11]);
                break;
            case "GAMECLEAR":
                audioSource.PlayOneShot(audioClips[12]);
                break;
        }
    }

}
