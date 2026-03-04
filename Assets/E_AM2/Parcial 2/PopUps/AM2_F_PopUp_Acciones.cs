using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM2_F_PopUp_Acciones : MonoBehaviour
{
    public AM2_F_PopUp Confirmaciones;
    public GameManager GM;

    //- Dejar de jugar y volver al menú principal
    public void VolverAlMenuInicial()
    {
        Confirmaciones.IniciarPoUpDeSeleccion("¿Seguro desea volver al menu inicial?", (seleccion) =>
            {
                if (seleccion == null) return;
                if (seleccion == true)
                {
                    GM.CambiarDeEscena("MainMenu");
                    Debug.Log("Se cambio a la escena MainMenu");
                }
                else
                {
                    Debug.Log("Selecciono q no");
                }
            }
        );
    }

    //- Borrar los datos de juego
    public void PedirBorrarLaData()
    {
        Confirmaciones.IniciarPoUpDeSeleccion("¿Seguro desea volver al menu inicial?", (seleccion) =>
            {
                if (seleccion == null) return;
                if (seleccion == true)
                {
                    PlayerPrefs.DeleteAll();
                    Debug.Log("Se borro toda la data");
                }
                else
                {
                    Debug.Log("Selecciono q no. Se anulo la borrada de data");
                }
            }
        );
    }





    /*
//- Comprar elementos de la tienda 
//- Obtener recompensas tras jugar un nivel
//- Opcional: Confirmación al intentar cerrar el juego
     */
}
