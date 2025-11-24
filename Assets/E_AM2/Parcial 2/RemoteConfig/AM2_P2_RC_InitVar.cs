using UnityEngine;

public class AM2_P2_RC_InitVar : MonoBehaviour
{
    [Header("Valores cargados desde Remote Config")]
    public int BalasMaximas;
    public float CoeficienteDeObtencionDeCurrency;
    public string Dificultad;
    public float VelocidadDeCaminar;
    public int VidaMaximaDelJugador;

    public void OnEnable()
    {
        InitAll();
    }

    // Llamar este método para inicializar las 5 variables
    public void InitAll()
    {
        // 1) BalasMaximas (int)
        AM2_P2_RC_Manager.GetInt("BalasMaximas", (val) =>
        {
            BalasMaximas = val;
            if (BalasMaximas == 0) Debug.LogWarning("BalasMaximas llegó 0 -> posible falta de key o valor 0 real.");
            Debug.Log("BalasMaximas = " + BalasMaximas);
        });

        // 2) CoeficienteDeObtencionDeCurrency (float)
        AM2_P2_RC_Manager.GetFloat("CoeficienteDeObtencionDeCurrency", (val) =>
        {
            CoeficienteDeObtencionDeCurrency = val;
            if (Mathf.Approximately(CoeficienteDeObtencionDeCurrency, 0f))
                Debug.LogWarning("CoeficienteDeObtencionDeCurrency llegó 0f -> posible falta de key o valor 0f real.");
            Debug.Log("CoeficienteDeObtencionDeCurrency = " + CoeficienteDeObtencionDeCurrency);
        });

        // 3) Dificultad (string)
        AM2_P2_RC_Manager.GetString("Dificultad", (val) =>
        {
            Dificultad = val;
            if (string.IsNullOrEmpty(Dificultad))
                Debug.LogWarning("Dificultad vacío -> posible falta de key o string vacío.");
            Debug.Log("Dificultad = " + Dificultad);
        });

        // 4) VelocidadDeCaminar (float)
        AM2_P2_RC_Manager.GetFloat("VelocidadDeCaminar", (val) =>
        {
            VelocidadDeCaminar = val;
            if (Mathf.Approximately(VelocidadDeCaminar, 0f))
                Debug.LogWarning("VelocidadDeCaminar llegó 0f -> posible falta de key o valor 0f real.");
            Debug.Log("VelocidadDeCaminar = " + VelocidadDeCaminar);
        });

        // 5) VidaMaximaDelJugador (int)
        AM2_P2_RC_Manager.GetInt("VidaMaximaDelJugador", (val) =>
        {
            VidaMaximaDelJugador = val;
            if (VidaMaximaDelJugador == 0) Debug.LogWarning("VidaMaximaDelJugador llegó 0 -> posible falta de key o valor 0 real.");
            Debug.Log("VidaMaximaDelJugador = " + VidaMaximaDelJugador);
        });
    }
}
