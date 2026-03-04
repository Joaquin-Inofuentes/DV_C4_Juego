using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GanarOPerder : MonoBehaviour
{
    public static GanarOPerder Instance;
    public int Puntos = 0;
    public TextMeshProUGUI ContadorDePuntos;


    public void Awake()
    {
        Instance = this;
    }

    public void OnEnable()
    {
        Awake();
    }

    public void AgregarPunto()
    {
        Puntos ++;
        ContadorDePuntos.text = $"{Puntos}/15";
        if(Puntos >= 3)
        {
            Ganar();
        }
    }

    public void Ganar()
    {
        GameManager.Instance.CambiarDeEscena("EscenaVictoria");
    }
}
