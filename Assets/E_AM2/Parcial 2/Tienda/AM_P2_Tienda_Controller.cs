using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM_P2_Tienda_Controller : MonoBehaviour
{
    [Button(nameof(AgregarCurrency),true)]
    public int Currency;

    public AM_P2_Tienda_Monedero Monedero;   // Currency Manager
    public AM_P2_Tienda_Items Items;        // Stats + costos
    public AM_P2_Tienda_View View;          // UI


    public void AgregarCurrency(int cantidad)
    {
        Monedero.AddCurrency(cantidad);
        View.ActualizarInterfaz();
    }



    // -------------------
    // COMPRAR DAÑO
    // -------------------
    public void ComprarDaño()
    {
        int costo = Items.CostoDañoProyectil;

        if (Monedero.GetCurrency() < costo)
            return; // no alcanza la moneda

        Monedero.SubtractCurrency(costo);
        Items.SumarDañoProyectil();
        Items.GuardarValores();
        View.ActualizarInterfaz();
    }

    // -------------------
    // COMPRAR VELOCIDAD RECARGA
    // -------------------
    public void ComprarVelocidadRecarga()
    {
        int costo = Items.CostoVelocidadDeRecarga;

        if (Monedero.GetCurrency() < costo)
            return;

        Monedero.SubtractCurrency(costo);
        Items.SumarVelocidadDeRecarga();
        Items.GuardarValores();
        View.ActualizarInterfaz();
    }

    // -------------------
    // COMPRAR SOLDADOS
    // -------------------
    public void ComprarSoldados()
    {
        int costo = Items.CostoSoldadosMaximos;

        if (Monedero.GetCurrency() < costo)
            return;

        Monedero.SubtractCurrency(costo);
        Items.SumarSoldadosMaximos();
        Items.GuardarValores();
        View.ActualizarInterfaz();
    }
}
