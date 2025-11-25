using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AM2_P2_RC_Manager ConfigRemote;
    public static GameManager Instance;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

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
}
