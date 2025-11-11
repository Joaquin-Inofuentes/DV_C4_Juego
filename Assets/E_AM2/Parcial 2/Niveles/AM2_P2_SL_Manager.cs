using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM2_P2_SL_Manager : MonoBehaviour
{
    public static event Action<string> Selecciono1Nivel;

    public void Seleccionar1Nivel(string nombreDeLaEscena)
    {
        Debug.Log($"🔹 Seleccionar1Nivel llamado con: {nombreDeLaEscena}");
        Selecciono1Nivel?.Invoke(nombreDeLaEscena);
    }
}
