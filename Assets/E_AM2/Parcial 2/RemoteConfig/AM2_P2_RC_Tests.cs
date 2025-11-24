using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using CustomInspector;

public class AM2_P2_RC_Tests : MonoBehaviour
{
    [Button(nameof(PedirStringEjemplo))]
    [Button(nameof(PedirIntEjemplo))]
    [Button(nameof(PedirFloatEjemplo))]
    public string ejemploString;
    public string ejemploInt;
    public string ejemploFloat;

    public void PedirStringEjemplo()
    {
        AM2_P2_RC_Manager.GetString(ejemploString, (value) =>
            {
            Debug.Log("Valor string recibido: " + value);
            });
    }
    public void PedirIntEjemplo()
    {
        AM2_P2_RC_Manager.GetInt(ejemploInt, (value) =>
            {
            Debug.Log("Valor int recibido: " + value);
            });
    }

    public void PedirFloatEjemplo()
    {
        AM2_P2_RC_Manager.GetFloat(ejemploFloat, (value) =>
            {
            Debug.Log("Valor float recibido: " + value);
            });
    }
}
