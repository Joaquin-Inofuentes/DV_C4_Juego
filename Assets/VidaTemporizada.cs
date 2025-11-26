using TMPro;
using UnityEngine;

public class VidaTemporizada : MonoBehaviour
{
    public float TiempoDeVida = 3f;   // Tiempo total
    private float timer;              // Timer interno
    public TextMeshProUGUI Texto;

    private void OnEnable()
    {
        timer = TiempoDeVida;        // Reinicia el timer
    }

    public void AsignarTexto(string texto)
    {
        Texto.text = texto;
    }

    private void Update()
    {
        timer -= Time.deltaTime;     // Descuenta el tiempo

        if (timer <= 0)
        {
            // Lo que quieras que pase al terminar
            gameObject.SetActive(false);
        }
    }
}
