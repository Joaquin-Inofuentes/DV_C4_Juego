using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM2_F_PopUp_Acciones : MonoBehaviour
{
    public AM2_F_PopUp Confirmaciones;
    public GameManager GM;
    public GameObject ObjetoARefrescar;

    //- Dejar de jugar y volver al menú principal
    public void VolverAlMenuInicial()
    {
        Confirmaciones.IniciarPoUpDeSeleccion("¿Seguro desea volver al menu inicial?", (seleccion) =>
            {
                if (seleccion == null) return;
                if (seleccion == true)
                {
                    GM.CambiarDeEscena("MainMenu");
                    Debug.Log("Se cambio a la escena MainMenu", gameObject);
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
        Confirmaciones.IniciarPoUpDeSeleccion("Estas a punto de borrar todo. ¿Seguro?", (seleccion) =>
            {
                if (seleccion == null) return;
                if (seleccion == true)
                {
                    Confirmaciones.IniciarPoUpDeSeleccion("¿Posta?, mira que no recuperaras tu tiempo invertido", (seleccion) =>
                    {
                        PlayerPrefs.DeleteAll();
                        Debug.Log("Se borro toda la data");
                        if (ObjetoARefrescar != null)
                        {
                            ObjetoARefrescar.SetActive(false);
                            ObjetoARefrescar.SetActive(true);
                            if (AM2_Stamina.instance != null)
                            {
                                AM2_Stamina.instance.Start();
                            }
                        }
                    });
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
