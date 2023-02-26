using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//地面のマテリアルをモンスターごとに変更するクラス
public class GroundMaterialController : MonoBehaviour
{
    [SerializeField] private Material[] _materials;

    // Update is called once per frame
    void Update()
    {
        switch(GameSystem.DeathMonsterNum)
        {
            case 0:
                GetComponent<MeshRenderer>().material = _materials[0];
                break;
            case 1:
                GetComponent<MeshRenderer>().material = _materials[1];
                break;
            case 2:
                GetComponent<MeshRenderer>().material = _materials[2];
                break;
            case 3:
                GetComponent<MeshRenderer>().material = _materials[3];
                break;
        }
    }
}
