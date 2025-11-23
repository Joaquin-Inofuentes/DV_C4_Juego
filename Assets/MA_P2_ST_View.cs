using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MA_P2_ST_View : MonoBehaviour
{
    public MA_P2_ST_PlayerArmas Armas;
    public TMPro.TMP_Text NombreArmaText;
    public TMPro.TMP_Text Balas;

    // Update is called once per frame
    void Update()
    {
        if (Armas != null)
        {
            NombreArmaText.text = 
                Armas.armaActual.Nombre.ToString();
            Balas.text = 
                Armas.armaActual.BalasActuales.ToString() 
                + " / " 
                + Armas.armaActual.MunicionTotal.ToString();
        }
    }
}
