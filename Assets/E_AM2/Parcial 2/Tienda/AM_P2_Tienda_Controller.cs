using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM_P2_Tienda_Controller : MonoBehaviour
{
    [Button(nameof(RestablecerTodo))]
    [Button(nameof(AgregarCurrency), true)]
    public int Currency = 1000;

    public AM_P2_Tienda_Monedero Monedero;   // Currency Manager
    public AM_P2_Tienda_Items Items;        // Stats + costos
    public AM_P2_Tienda_View View;          // UI

    public AM2_P2_AdsManager adsManager;

    public VidaTemporizada MensajeTemporal;

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
        {
            MensajeTemporal.gameObject.SetActive(true);
            return; // no alcanza la moneda
        }

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
        {
            MensajeTemporal.gameObject.SetActive(true);
            return; // no alcanza la moneda
        }

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
        {
            MensajeTemporal.gameObject.SetActive(true);
            return; // no alcanza la moneda
        }

        Monedero.SubtractCurrency(costo);
        Items.SumarSoldadosMaximos();
        Items.GuardarValores();
        View.ActualizarInterfaz();
    }


    public void RestablecerTodo()
    {
        // Reset de Monedero
        Monedero.ResetCurrency();

        // Reset de Items
        Items.DañoProyectil = 0;
        Items.VelocidadDeRecarga = 0;
        Items.SoldadosMaximos = 0;

        // Guardar reset en PlayerPrefs
        Items.GuardarValores();

        // Actualizar UI
        View.ActualizarInterfaz();
    }


    public AudioSource SonidoDeDineroConseguido;
    public void ObtenerCurrencyViaAds()
    {
        AM2_P2_AdsManager.Instance.ShowRewarded(result =>
        {
            Debug.Log("RESULTADO DEL AD: " + result);
            MensajeTemporal.gameObject.SetActive(true);

            if (result == AdsResult.Completed)
            {
                MensajeTemporal.AsignarTexto("¡¡ Ganaste 1000 euros !!");
                AgregarCurrency(1000);
                Debug.Log("El jugador COMPLETÓ el anuncio ✔");
                if(SonidoDeDineroConseguido != null)
                {
                    SonidoDeDineroConseguido.Play();
                }
            }
            else if (result == AdsResult.Skipped)
            {
                MensajeTemporal.AsignarTexto("No debiste saltearte...");
                Debug.Log("El jugador LO SALTÓ ⏭");
            }
            else if (result == AdsResult.Failed)
            {
                MensajeTemporal.AsignarTexto("Falla del server. Re intente");
                Debug.Log("El anuncio FALLÓ ❌");
            }
        });
    }
}
