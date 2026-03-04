using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

public class GameManager : MonoBehaviour
{
    public GameObject PanelDeCargandoPantalla;

    public AM2_P2_RC_Manager ConfigRemote;
    public static GameManager Instance;

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
        if (PanelDeCargandoPantalla != null)
            PanelDeCargandoPantalla.SetActive(true);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
        if (AM2_P2_AdsManager.Instance != null)
            AM2_P2_AdsManager.Instance.ShowBanner();
        EscenaCargandose = nombreEscena;
        Invoke(nameof(CargarEscenaAsyncDesactivada), 1);
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
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EscenaCargandose);
        op.allowSceneActivation = true;
    }
}
