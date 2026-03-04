using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AccionAlDestruir : MonoBehaviour
{
    public UnityEvent onDestroyed;
    public void OnDestroy()
    {
        Debug.Log("Se llamo a ondstroy desde " + gameObject.name, gameObject);
        Debug.Log("Se perdio. Pero tengo el bug de q me cambia de escena por q cuando desactivo esta escena llama a estos");
        //if (GameManager.SeEstaCargandoUnaEscena == true)
            //onDestroyed.Invoke();
    }
}
