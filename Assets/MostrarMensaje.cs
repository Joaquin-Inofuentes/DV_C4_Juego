using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MostrarMensaje : MonoBehaviour
{
    public static MostrarMensaje instancia;
    public GameObject Mensaje;
    public TextMeshProUGUI textoMensaje;

    public float TiempoRestante = 2;
    public float TiempoParaDesaparecer = 2f;

    // Update is called once per frame
    void Update()
    {
        if (TiempoRestante > 0)
        {
            TiempoRestante -= Time.deltaTime;
            if (TiempoRestante <= 0 && TiempoRestante > -5)
            {
                Mensaje.SetActive(false);
                TiempoRestante = -10;
            }
        }
    }

    public static void Mostrar(string mensaje)
    {
        if (instancia != null)
        {
            instancia.Mensaje.SetActive(true);
            instancia.textoMensaje.text = mensaje;
            instancia.TiempoRestante = instancia.TiempoParaDesaparecer;
        }
    }

}
