using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalVariables : MonoBehaviour
{
    //Œ»İ‚ÌHP
    static public float currentHP = 3000;

    // Use this for initialization
    void Start()
    {
        VariableReset();
    }

    static public void VariableReset() //•Ï”‰Šú‰»
    {
        currentHP = 3000;
    }
}