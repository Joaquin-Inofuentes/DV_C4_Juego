using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM_P2_Tienda_Items : MonoBehaviour
{
    public int DañoProyectil = 0;
    public int VelocidadDeRecarga = 0;
    public int SoldadosMaximos = 0;

    public int CostoDañoProyectil = 300;
    public int CostoVelocidadDeRecarga = 250;
    public int CostoSoldadosMaximos = 350;


    public AM_P2_Tienda_View View;

    private void OnEnable()
    {
        CargarValores();
    }

    // Métodos para sumar 1
    public void SumarDañoProyectil()
    {
        DañoProyectil += 1;
        GuardarValores();
    }

    public void SumarVelocidadDeRecarga()
    {
        VelocidadDeRecarga += 1;
        GuardarValores();
    }

    public void SumarSoldadosMaximos()
    {
        SoldadosMaximos += 1;
        GuardarValores();
    }

    // Guardar todos los valores en PlayerPrefs
    public void GuardarValores()
    {
        PlayerPrefs.SetInt("DañoProyectil", DañoProyectil);
        PlayerPrefs.SetInt("VelocidadDeRecarga", VelocidadDeRecarga);
        PlayerPrefs.SetInt("SoldadosMaximos", SoldadosMaximos);
        PlayerPrefs.Save();
    }

    // Cargar valores desde PlayerPrefs
    public void CargarValores()
    {
        DañoProyectil = PlayerPrefs.GetInt("DañoProyectil", DañoProyectil);
        VelocidadDeRecarga = PlayerPrefs.GetInt("VelocidadDeRecarga", VelocidadDeRecarga);
        SoldadosMaximos = PlayerPrefs.GetInt("SoldadosMaximos", SoldadosMaximos);
        View?.ActualizarInterfaz();
        AplicarCambios();
    }

    public C_Shooter Arma;
    public AM2_P2_Aliado Arma2;
    public C_SoldadoJugador Soldado;
    public void AplicarCambios()
    {
        if (Arma == null)
        {
            Debug.LogWarning("Estas en el menu");
            return;
        }
        if (DañoProyectil > 0)
            Arma.DamageProyectil = 5 * DañoProyectil;
        if (SoldadosMaximos > 0)
            Soldado.CantidadMaximaDeAliados = 7 + SoldadosMaximos;
        if (VelocidadDeRecarga > 0)
            Arma2.tiempoRecarga = 1.2f / VelocidadDeRecarga;
    }
}
