using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AM2_F_GanarOPerder : MonoBehaviour
{
    public static AM2_F_GanarOPerder Instance;
    public int Puntos = 0;
    public TextMeshProUGUI ContadorDePuntos;
    public int PuntosParaGanar = 3;

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
        ContadorDePuntos.text = $"{Puntos}/{PuntosParaGanar}";
        if(Puntos >= PuntosParaGanar)
        {
            Ganar();
        }
    }

    public void Ganar()
    {
        GameManager.Instance.CambiarDeEscena("EscenaVictoria");
    }
}
