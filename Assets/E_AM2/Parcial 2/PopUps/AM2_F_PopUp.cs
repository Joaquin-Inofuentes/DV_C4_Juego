using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AM2_F_PopUp : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject Fondo;

    [SerializeField] private TextMeshProUGUI TextoDePregunta;

    [SerializeField] private Button BotonDeSi;
    [SerializeField] private Button BotonDeNo;
    [SerializeField] private Button BotonDeCancelar;
    [SerializeField] private Button BotonDeCancelar2;

    private Action<bool?> _callback;

    public void IniciarPoUpDeSeleccion(string Pregunta, Action<bool?> action)
    {
        TextoDePregunta.text = Pregunta;

        _callback = action;

        Panel.SetActive(true);
        Fondo.SetActive(true);

        BotonDeSi.onClick.RemoveAllListeners();
        BotonDeNo.onClick.RemoveAllListeners();
        BotonDeCancelar.onClick.RemoveAllListeners();
        BotonDeCancelar2.onClick.RemoveAllListeners();

        BotonDeSi.onClick.AddListener(() => Responder(true));
        BotonDeNo.onClick.AddListener(() => Responder(false));
        BotonDeCancelar.onClick.AddListener(() => Responder(null));
        BotonDeCancelar2.onClick.AddListener(() => Responder(null));
    }

    private void Responder(bool? resultado)
    {
        Panel.SetActive(false);
        Fondo.SetActive(false);

        _callback?.Invoke(resultado);
    }



    public void PreguntarSalirDelJuego()
    {
        IniciarPoUpDeSeleccion("¿Seguro que queres salir del juego?", (res) =>
        {
            if (res == true)
            {
                SalirDelJuego();
            }
        });
    }

    private void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }


    /*
- Comprar elementos de la tienda 
- Borrar los datos de juego
- Obtener recompensas tras jugar un nivel
- Dejar de jugar y volver al menú principal
- Opcional: Confirmación al intentar cerrar el juego
     */
}