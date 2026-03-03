using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_F_ControllerSeguidor : MonoBehaviour
{
    public GameObject Posicion;
    public GameObject target; // El objetivo a seguir
    public IA_P2_AgentIA agent; // Referencia al agente para moverlo
    public List<GameObject> Enemigos; // Lista de enemigos para detectar
    public LayerMask obstacleLayer; // Capa de obstáculos para el raycast
    public IA_F_EnemyCercanos Cercanos;
    void Update()
    {
        target = ObtenerEnemigo();

        if (Posicion == null) return;
        if (target != null )
        {
            agent.GoTo(target.transform.position);
        }
        else
        {
            if (Posicion != null)
            {
                agent.GoTo(Posicion.transform.position);
            }
        }

    }

    private GameObject ObtenerEnemigo()
    {
        Enemigos = Cercanos.Colisionados;

        List<GameObject> enemigosVisibles = new List<GameObject>();
        List<int> IndicesAEliminar = new List<int>();
        foreach (var enemigo in Enemigos)
        {
            if (enemigo == null)
            {
                IndicesAEliminar.Add(Enemigos.IndexOf(enemigo)); // Guardamos el índice del enemigo destruido para eliminarlo después
                continue; // Si el enemigo ha sido destruido, lo saltamos
            }
            Transform PosibleObjetivo = enemigo.transform;
            if (EsVisible(PosibleObjetivo))
            {
                enemigosVisibles.Add(enemigo);
                Debug.DrawLine(enemigo.transform.position, agent.transform.position,Color.blue);
            }
        }
        // Eliminamos los enemigos destruidos de la lista después de iterar para evitar problemas de modificación durante la iteración
        foreach (var indice in IndicesAEliminar)
        {
            Enemigos.RemoveAt(indice);
        }
        // Ordenamos de mas lejos a mas cercano a este
        Vector3 PosicionPropia = agent.transform.position;
        enemigosVisibles.Sort((a, b) =>
            Vector3.Distance(PosicionPropia, a.transform.position).CompareTo(
                Vector3.Distance(PosicionPropia, b.transform.position)));
        // Seleccionamos el mas cercano
        GameObject enemigoMasCercano = Enemigos.Count > 0 ? Enemigos[0] : null;

        return enemigoMasCercano;
    }

    public bool EsVisible(Transform PosibleEnemigo)
    {
        Transform PosicionAgente = agent.transform;

        // Dirección desde el agente hacia el posible enemigo
        Vector3 direccion = PosibleEnemigo.position - PosicionAgente.position;

        // Distancia al posible enemigo
        float distancia = Vector3.Distance(PosicionAgente.position, PosibleEnemigo.position);

        // Realizamos un Raycast para verificar si hay obstáculos entre el agente y el posible enemigo
        if (!Physics.Raycast(PosicionAgente.position, direccion.normalized, distancia, obstacleLayer))
        {
            // Si no hay colisión con un obstáculo, el enemigo es visible
            return true;
        }

        // Si hay colisión con un obstáculo, el enemigo no es visible
        return false;
    }
}
