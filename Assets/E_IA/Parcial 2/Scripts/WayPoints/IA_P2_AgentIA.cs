using CustomInspector;
using System;
using System.Collections.Generic;
using TMPro;
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

    public List<Vector3> currentPath;
    public int currentIndex = 0;
    public bool isMoving = false;
    public float currentSpeed = 0f;

    public float DistanceStop = 1f;

    public void OnDisable()
    {
        isMoving = false;
        currentPath = null;
        currentIndex = 0;
        currentSpeed = 0f;
    }

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
        targetPosition.y = 0;
        int Estado = GetStateActual(targetPosition);
        if (Estado == 1) // Visible
        {
            // Limpiar waypoints
            currentPath = null;
            currentIndex = 0;

            // Ir directo sin pensar camino
            currentPath = new List<Vector3> { targetPosition };
            isMoving = true;
            currentSpeed = 0f;

            return;
        }

        if (Estado == 2) // No visible pero el ultimo waypoint lo ve
        {
            // Actualiza la ultima pos
            currentPath[currentPath.Count - 1] = targetPosition;
            return;
        }

        Vector3 Origen = transform.position;
        Origen.y = 0;
        targetPosition.y = 0;

        var model = IA_P2_PathfindingModel.Instance;
        LayerMask obstacleLayer = model.obstacleLayer;

        List<Vector3> RecorridoAStar = IA_P2_PathfindingManager.RequestPath(Origen, targetPosition, Offset);

        List<Vector3> RecorridoConTheta =
            IA_F_PathFinding_Theta.OptimizarConTheta(
                RecorridoAStar,
                obstacleLayer
            );

        currentPath = RecorridoAStar;

        /*
        if (currentPath != null && currentPath.Count > 1)
        {
            for (int i = 0; i < currentPath.Count - 1; i++)
                Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.cyan, 3f);
        }
        */
        currentIndex = 0;
        isMoving = currentPath != null && currentPath.Count > 0;
        currentSpeed = 0f;
    }

    public int GetStateActual(Vector3 targetPosition)
    {
        Vector3 PosAAnalizar = transform.position;
        var model = IA_P2_PathfindingModel.Instance;
        LayerMask obstacleLayer = model.obstacleLayer;

        // Si, desde mi pos veo al objetivo
        if (!Physics.Linecast(PosAAnalizar, targetPosition, obstacleLayer))
        {
            //Debug.Log("Es visible desde la pos actual");
            return 1; // Camino directo visible
        }
        // Obtengo el último waypoint su posición
        if (currentPath != null && currentPath.Count > 0)
        {
            if (currentPath.Count > 2)
            {
                Vector3 UltimoWaypoint = currentPath[currentPath.Count - 2];

                // Trazo un rayo del último waypoint al target
                if (!Physics.Linecast(UltimoWaypoint, targetPosition, obstacleLayer))
                {
                    //Debug.Log("El camino aun sirve");
                    //Debug.DrawLine(UltimoWaypoint, targetPosition, Color.yellow, 2);
                    return 2; // Camino visible desde el último waypoint
                }
            }
        }


        return 0; // Camino no sirve
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
            // Rotacion instantanea
            //transform.rotation = targetRotation;
            // Rotacion suave
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
                //Debug.DrawLine(transform.position, target, Color.red);
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


    public bool IsOnFinalPathSegment()
    {
        if (currentPath == null || currentPath.Count == 0)
            return false;

        if (currentPath.Count == 1)
            return true;

        if (currentPath == null || currentPath.Count < 2)
            return false;

        return currentIndex == currentPath.Count;
    }

    public void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // opcional: evita inclinarse hacia arriba/abajo

        if (direction.sqrMagnitude < 0.0001f)
            return; // evita errores de LookRotation

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }













    static List<Vector3> OptimizarConTheta(List<Vector3> RecorridoAStar)
    {
        // Primero revisa si 

        return null;
    }


    /* Usa esto
     

IA_P2_LineOfSight3D. public static bool Check(Vector3 from, Vector3 to,LayerMask obstacleLayer)
    {
        from.y = 0;
        to.y = 0;
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        return !Physics.Raycast(from, dir.normalized, dist, obstacleLayer);
    }


     */

}