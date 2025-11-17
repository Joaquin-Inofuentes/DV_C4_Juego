using CustomInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_AgentIA : MonoBehaviour
{
    [Button(nameof(GoToGameobject), true)]
    public GameObject targetObject;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f; 
    public float nodeReachDistance = 0.5f;

    [Header("Debug")]
    public bool debug_BlockMovement = false;
    public bool debug_BlockRotation = false; 

    // --- SECCIÓN DE DETECCIÓN (ELIMINADA) ---
    // (Aquí estaban fieldOfView, viewDistance, etc.)

    public List<Vector3> currentPath;
    public int currentIndex = 0;
    public bool isMoving = false;
    public float currentSpeed = 0f;

    public float DistanceStop = 1f;

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public void AsignarColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }

    public void GoToGameobject(GameObject target)
    {
        GoTo(target.transform.position);
    }

    public void GoTo(Vector3 targetPosition, float Offset = 0)
    {
        Vector3 Origen = transform.position;
        Origen.y = 0;
        targetPosition.y = 0;
        currentPath = IA_P2_PathfindingManager.RequestPath(Origen, targetPosition, Offset);

        if (currentPath != null && currentPath.Count > 1)
        {
            Debug.DrawLine(transform.position, currentPath[0], Color.red, 4f);
        }

        currentIndex = 0;
        isMoving = currentPath != null && currentPath.Count > 0;
        currentSpeed = 0f;

        if (currentPath == null || currentPath.Count < 2) return;

        for (int i = 0; i < currentPath.Count - 1; i++)
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.cyan, 3f);
    }

    void Update()
    {
        // 1. Salida si no nos movemos o no hay camino
        if (!isMoving || currentPath == null || currentPath.Count == 0)
            return;

        // 2. Obtener el objetivo actual del camino
        Vector3 target = currentPath[currentIndex];
        Vector3 toTarget = target - transform.position;
        float distance = toTarget.magnitude;

        // 3. Lógica de Rotación
        // Solo rota si no está bloqueado y si no está ya en el destino (evita error de LookRotation)
        if (!debug_BlockRotation && distance > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized);
            // Usa Slerp para una rotación suave
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // 4. Lógica de Movimiento
        // Salir si el movimiento está bloqueado por debug
        if (debug_BlockMovement)
            return;

        // Comprobar si hemos llegado al nodo actual
        if (distance <= nodeReachDistance)
        {
            // Estamos "en" el nodo, pasamos al siguiente
            currentIndex++;
            if (currentIndex >= currentPath.Count)
            {
                // Llegamos al final del camino
                isMoving = false;
                transform.position = target; // Opcional: "snap" a la posición final
            }
        }
        else
        {
            // Aún no llegamos, moverse hacia el nodo
            float step = moveSpeed * Time.deltaTime;
            transform.position += toTarget.normalized * step;
        }

        // 5. Lógica de Dibujo de Líneas (sin cambios)
        int lastIndex = currentPath.Count - 1;

        // Tramos completados: amarillo
        for (int i = 0; i < currentIndex - 1; i++)
        {
            if (currentPath.Count > i + 1)
                Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.yellow, 3f);
        }

        // Tramos por recorrer: blanco
        for (int i = Mathf.Max(currentIndex - 1, 0); i < lastIndex; i++)
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.white, 0.1f);
    }

    public void StopAgent()
    {
        isMoving = false;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

}