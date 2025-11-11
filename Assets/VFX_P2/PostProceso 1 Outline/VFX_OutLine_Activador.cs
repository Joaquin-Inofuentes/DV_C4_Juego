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

    void OnMouseEnter()
    {
        // Solo activar si el objeto existe Y está actualmente desactivado
        if (ObjetoDeFondo != null && !ObjetoDeFondo.activeSelf)
        {
            ObjetoDeFondo.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        // Solo desactivar si el objeto existe Y está actualmente activado
        if (ObjetoDeFondo != null && ObjetoDeFondo.activeSelf)
        {
            ObjetoDeFondo.SetActive(false);
        }
    }
}