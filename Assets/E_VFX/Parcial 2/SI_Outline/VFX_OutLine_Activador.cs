using CustomInspector;
using UnityEngine;

public class VFX_OutLine_Activador : MonoBehaviour, VFX_P2_INT_Apuntable
{
    [Button(nameof(RecojerArma))]
    public GameObject ObjetoDeFondo;

    public C_SoldadoJugador SoldadoJugador;
    public bool Recogido = false;

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
        if (SoldadoJugador == null) return;
        if (Input.GetKeyDown(KeyCode.E) && !Recogido)
        {
            if (Vector3.Distance(SoldadoJugador.transform.position, transform.position) < 3f)
            {
                Recogido = true;
                RecojerArma();
            }
        }
    }
    public GameObject LuzDeFondo;
    public void RecojerArma()
    {
        SoldadoJugador.Recojer(transform);
        LuzDeFondo.SetActive(false);
    }

    public void OnMouseOver()
    {
        Activar();
    }

    public void OnMouseExit()
    {
        Desactivar();
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

    public void Apuntado()
    {
        // Falto implementar este método
        throw new System.NotImplementedException();
    }
}