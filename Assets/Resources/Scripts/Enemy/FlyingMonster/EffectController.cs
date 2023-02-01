using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//もしもパーティクルシステムの再生が終了していたら自身を消す
public class EffectController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(GetComponent<ParticleSystem>() == null)
        //{
        //    return;
        //}

        //if (GetComponentInChildren<ParticleSystem>().isStopped)
        //{
        //    //foreach (GameObject child in this.transform)
        //    //{
        //    //    // 一つずつ破棄する
        //    //    Destroy(child.gameObject);
        //    //}
        //}
    }
}
