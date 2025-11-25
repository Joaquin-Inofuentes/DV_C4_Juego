using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetivoFinal : MonoBehaviour
{
    public static GameObject Objetivo;
    public void OnEnable()
    {
        Objetivo = this.gameObject;
    }

}
