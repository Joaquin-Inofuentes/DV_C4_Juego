using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

public class IA_F_ChangeMode : MonoBehaviour
{
    [Button(nameof(OrdenarAtaque))]
    public IA_P2_AgentIA agentIA;
    public C_SoldadoJugador soldadoJugador;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D)
            )
        {
            CambiarDeModo(true);
        }

        // Detecta click izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            CambiarDeModo(false);
            // Obtiene posicion
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Mueve el agente a la posicion del click
                agentIA.GoTo(hit.point);
                agentIA.LookAtTarget(hit.point);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            CambiarDeModo(false);
            // Obtiene posicion
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Mueve el agente a la posicion del click
                agentIA.LookAtTarget(hit.point);
            }
        }




    }


    public void CambiarDeModo()
    {
        if (agentIA == null || soldadoJugador == null)
        {
            Debug.LogWarning("AgentIA o SoldadoJugador no asignados.");
            return;
        }

        if (agentIA.enabled)
        {
            agentIA.enabled = false;
            soldadoJugador.enabled = true;
            Debug.Log("Modo Jugador activado.");
        }
        else
        {
            agentIA.enabled = true;
            soldadoJugador.enabled = false;
            Debug.Log("Modo IA activado.");
        }
    }
    public void CambiarDeModo(bool ModoIA)
    {
        if (agentIA == null || soldadoJugador == null)
        {
            Debug.LogWarning("AgentIA o SoldadoJugador no asignados.");
            return;
        }

        if (ModoIA)
        {
            agentIA.enabled = false;
            soldadoJugador.enabled = true;
            //Debug.Log("Modo Jugador activado.");
        }
        else
        {
            agentIA.enabled = true;
            soldadoJugador.enabled = false;
            //Debug.Log("Modo IA activado.");
        }
    }




    public GameObject objetivoEnemigo;

    [ContextMenu("Forzar Ataque Global")]
    public void OrdenarAtaque()
    {
        // Esto activará a TODOS los agentes que tengan el script IA_P2_FSM
        IA_P2_BusEvent_Manager.NotificarEncontrado(objetivoEnemigo);
    }
}
