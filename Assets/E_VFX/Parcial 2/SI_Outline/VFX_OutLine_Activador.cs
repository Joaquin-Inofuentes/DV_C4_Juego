using UnityEngine;

public class VFX_OutLine_Activador : MonoBehaviour
{
    public GameObject ObjetoDeFondo;

    void Start()
    {
        // Solo desactivar si el objeto existe Y está actualmente activado
        if (ObjetoDeFondo != null && ObjetoDeFondo.activeSelf)
        {
            ObjetoDeFondo.SetActive(false);
        }
    }

    public void Update()
    {
        // Esto es solo para pruebas en el editor, se puede eliminar en producción
        if (Input.GetKeyDown(KeyCode.O))
        {
            Activar();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Desactivar();
        }
    }

    void Activar()
    {
        // Solo activar si el objeto existe Y está actualmente desactivado
        if (ObjetoDeFondo != null && !ObjetoDeFondo.activeSelf)
        {
            Debug.Log("Se activo el outline");
            ObjetoDeFondo.SetActive(true);
        }
    }

    void Desactivar()
    {
        // Solo desactivar si el objeto existe Y está actualmente activado
        if (ObjetoDeFondo != null && ObjetoDeFondo.activeSelf)
        {
            Debug.Log("Se desactivo el outline");
            ObjetoDeFondo.SetActive(false);
        }
    }
}