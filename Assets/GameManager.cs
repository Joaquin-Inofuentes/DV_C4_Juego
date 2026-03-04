using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

public class GameManager : MonoBehaviour
{
    public GameObject PanelDeCargandoPantalla;

    public AM2_P2_RC_Manager ConfigRemote;
    public static GameManager Instance;

    public static bool SeEstaCargandoUnaEscena;

    public void Start()
    {
        AM2_P2_AdsManager.Instance.HideBanner();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CambiarDeEscena(string nombreEscena)
    {
        SeEstaCargandoUnaEscena = true;
        Debug.Log("Se recibio el cambio de escena a " + nombreEscena);
        if (PanelDeCargandoPantalla != null)
            PanelDeCargandoPantalla.SetActive(true);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
        if (AM2_P2_AdsManager.Instance != null)
            AM2_P2_AdsManager.Instance.ShowBanner();
        Debug.Log("Espere 1 segundo");
        EscenaCargandose = nombreEscena;
        CargarEscenaAsyncDesactivada();
        //CargarEscenaAsyncDesactivada();
    }

    public static string EscenaCargandose = "";

    public void ReiniciarEscenaActual()
    {
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaActual);
    }


    public void BorrarDatos()
    {
        Debug.Log("Borrar datos");
    }

    public void CargarDatos()
    {
        Debug.Log("Cargar datos");
    }

    public void GuardarDatos()
    {
        Debug.Log("Guardar datos");
    }

    public void IterarActivacion(GameObject Objeto) // Interaccion con botones
    {
        Objeto.SetActive(Objeto.activeSelf);
    }



    public void CargarEscenaAsyncDesactivada()
    {
        Debug.Log("Inicio la escena con " + EscenaCargandose);
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EscenaCargandose);
        if (op != null)
            op.allowSceneActivation = true;
    }
}
