using UnityEngine;
using TMPro;

public class AM2_P2_RC_Viewer : MonoBehaviour
{
    [Header("Referencia al inicializador de variables")]
    public AM2_P2_RC_InitVar init;

    [Header("Salida visual (TMP UGUI)")]
    public TMP_Text output;

    void OnEnable()
    {
        // Espera 2 segundos y luego muestra los valores
        Invoke(nameof(VolcarValoresEnPantalla), 2f);
    }

    public void VolcarValoresEnPantalla()
    {
        if (init == null)
        {
            Debug.LogError("Viewer → Sin referencia al inicializador.");
            if (output != null) output.text = "ERROR: No hay referencia a Init.";
            return;
        }

        if (output == null)
        {
            Debug.LogError("Viewer → Falta asignar el TMP_Text.");
            return;
        }

        // Construimos el texto de salida
        output.text =
            "=== Remote Config ===\n" +
            $"Balas Maximas: {init.BalasMaximas}\n" +
            $"Coef. Currency: {init.CoeficienteDeObtencionDeCurrency}\n" +
            $"Dificultad: {init.Dificultad}\n" +
            $"Velocidad Caminar: {init.VelocidadDeCaminar}\n" +
            $"Vida Maxima: {init.VidaMaximaDelJugador}\n" +
            "=====================";
    }
}
