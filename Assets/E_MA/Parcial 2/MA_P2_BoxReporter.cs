using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MA_P2_BoxReporter : MonoBehaviour
{

    public UnityEvent<Collider> AccionesAlEntrar;
    public UnityEvent<Collider> AccionesAlSalir;

    public UnityEvent<Collider> AccionasAlEstarDentro;
    public float LlamadoPorSegundo = 1f;

    private float timer;
    // Creamos una lista para recordar quiénes están dentro
    private List<Collider> objetosDentro = new List<Collider>();

    void Update()
    {
        // 1. Controlamos el tiempo
        timer += Time.deltaTime;

        if (timer >= LlamadoPorSegundo)
        {
            // 2. Si llegó el momento, disparamos el evento A TODOS los que estén en la lista
            // Usamos un bucle inverso por seguridad (por si alguno se destruye al recibir el evento)
            for (int i = objetosDentro.Count - 1; i >= 0; i--)
            {
                if (objetosDentro[i] != null)
                {
                    AccionasAlEstarDentro.Invoke(objetosDentro[i]);
                }
                else
                {
                    // Limpieza: Si el objeto fue destruido, lo sacamos de la lista
                    objetosDentro.RemoveAt(i);
                }
            }

            timer = 0f; // Reiniciamos el reloj
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!objetosDentro.Contains(other))
        {
            objetosDentro.Add(other);
        }
        AccionesAlEntrar.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (objetosDentro.Contains(other))
        {
            objetosDentro.Remove(other);
        }
        AccionesAlSalir.Invoke(other);
    }


}
