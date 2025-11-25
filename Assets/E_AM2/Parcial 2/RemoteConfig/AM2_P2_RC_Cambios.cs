using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM2_P2_RC_Cambios : MonoBehaviour
{
    public AM2_P2_RC_InitVar InicializadorDeRC;

    public void OnEnable()
    {
        InicializadorDeRC.OnAllValuesLoaded += ActionAlRecibirParametros;
    }
    public AM2_P2_Aliado Jugador;
    public C_SoldadoJugador JugadorC;
    public Destruible DestruibleVida;
    public void ActionAlRecibirParametros()
    {
        int BalasMaximas = InicializadorDeRC.BalasMaximas;
        Jugador.balasMaximas = BalasMaximas;
        
        float CoeficienteDeObtencionDeCurrency = InicializadorDeRC.CoeficienteDeObtencionDeCurrency;
        PlayerPrefs.SetFloat("CoeficienteDeObtencionDeCurrency", CoeficienteDeObtencionDeCurrency);
        
        string Dificultad = InicializadorDeRC.Dificultad;
        PlayerPrefs.SetString("Dificultad", Dificultad);
        // Lo leen los enemigos al crearse para saber su vida y daño
        // AM2_P2_Enemigo.cs >> OnEnable()
        // Codigo : 5004

        float VelocidadDeCaminar = InicializadorDeRC.VelocidadDeCaminar;
        JugadorC.velocidad = VelocidadDeCaminar * 2;

        int VidaMaxima = InicializadorDeRC.VidaMaximaDelJugador;
        DestruibleVida.healthMax = VidaMaxima;
        DestruibleVida.health = VidaMaxima;
        PlayerPrefs.Save();
        Debug.Log("Se guardo lo de player prefabs");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
