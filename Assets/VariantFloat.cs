using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VariantFloat{
    public float baseValue;
    public float variance;
    public float Evaluate()
    {
        return baseValue + Random.Range(-variance, variance);
    }
}
