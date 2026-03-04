using UnityEngine;
using System;

public class AM2_P2_RC_InitVar : MonoBehaviour
{
    [Header("Valores cargados desde Remote Config")]
    public int BalasMaximas;
    public float CoeficienteDeObtencionDeCurrency;
    public string Dificultad;
    public float VelocidadDeCaminar;
    public int VidaMaximaDelJugador;

    public Action OnAllValuesLoaded;

    private int counter = 0;
    private const int totalRequests = 5;
    private bool alreadyInvoked = false;

    public void OnEnable()
    {
        counter = 0;
        alreadyInvoked = false;
        InitAll();
    }

    private void CheckComplete()
    {
        counter++;
        if (counter >= totalRequests && !alreadyInvoked)
        {
            alreadyInvoked = true;
            OnAllValuesLoaded?.Invoke();
        }
    }

    public bool FalloLaObtencion = false;
    public void InitAll()
    {
        AM2_P2_RC_Manager.GetInt("BalasMaximas", (val) =>
        {
            if (val == 0) FalloLaObtencion |= true;
            if (FalloLaObtencion) return;
            BalasMaximas = val;
            Debug.Log("BalasMaximas = " + BalasMaximas);
            CheckComplete();
        });

        AM2_P2_RC_Manager.GetFloat("CoeficienteDeObtencionDeCurrency", (val) =>
        {
            if (val == 0) FalloLaObtencion |= true;
            if (FalloLaObtencion) return;
            CoeficienteDeObtencionDeCurrency = val;
            Debug.Log("CoeficienteDeObtencionDeCurrency = " + CoeficienteDeObtencionDeCurrency);
            CheckComplete();
        });

        AM2_P2_RC_Manager.GetString("Dificultad", (val) =>
        {
            if (val == "") FalloLaObtencion |= true;
            if (FalloLaObtencion) return;
            Debug.Log("Dificultad = " + Dificultad);
            CheckComplete();
        });

        AM2_P2_RC_Manager.GetFloat("VelocidadDeCaminar", (val) =>
        {
            if (val == 0) FalloLaObtencion |= true;
            if (FalloLaObtencion) return;
            Debug.Log("VelocidadDeCaminar = " + VelocidadDeCaminar);
            CheckComplete();
        });

        AM2_P2_RC_Manager.GetInt("VidaMaximaDelJugador", (val) =>
        {
            if (val == 0) FalloLaObtencion |= true;
            if (FalloLaObtencion) return;
            Debug.Log("VidaMaximaDelJugador = " + VidaMaximaDelJugador);
            CheckComplete();
        });
    }
}
